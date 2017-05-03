
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ServerTcpChat.Classes;
using System.Net;
using System.Net.Sockets;
using CommonChatTypes;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace ServerTcpChat.Classes
{
    public delegate bool RemoveWorker(int p_thred_id);


    public static class WorkerThread
    {
        public static void WorkerMainThread(ServerWorkerData p_worker_data, int p_port_num, int p_thread_id, object p_all_treads_data_lock, IPAddress p_server_ip_address)
        {
            ServerWorkerData worker_data = p_worker_data;
            int port_num = p_port_num;
            int thread_id = p_thread_id;

            ServerWorkerStatus status = ServerWorkerStatus.WaithForAclinet;
            System.Timers.Timer worker_timer_object = new System.Timers.Timer();
            worker_timer_object.Interval = 12500;
            worker_timer_object.AutoReset = false;

            worker_timer_object.Elapsed += new System.Timers.ElapsedEventHandler((sender, e) => worker_timer_Elapsed(sender, e, worker_data.send_to_worker_construct
                , worker_data.worker_pulse_object));
            WorkerTimer timer = new WorkerTimer(worker_timer_object);

            Socket client_socket = null;
            IPEndPoint server_endpoint = new IPEndPoint(p_server_ip_address, port_num);
            TcpListener server_listener = new TcpListener(server_endpoint);
            try
            {
                System.Timers.Timer listener_timer = new System.Timers.Timer(10000);
                listener_timer.AutoReset = false;
                bool timer_elapsed = false;
                listener_timer.Elapsed += new System.Timers.ElapsedEventHandler((sender, e) => listener_timer_Elapsed(sender, e, out timer_elapsed));
                server_listener.Start(1);
                listener_timer.Start();
                while (true)
                {
                    Thread.Sleep(500);
                    if (server_listener.Pending())
                    {
                        client_socket = server_listener.AcceptSocket();
                        break;
                    }
                    if (timer_elapsed)
                    {
                        try
                        {
                            client_socket.Shutdown(SocketShutdown.Both);
                            client_socket.Close();
                        }
                        catch
                        {
                        }
                        lock (p_all_treads_data_lock)
                        {
                            worker_data.remove_worker(p_thread_id);
                        }
                        Thread.CurrentThread.Abort();
                        return;
                    }
                }

            }
            catch (Exception)
            {
                try
                {
                    client_socket.Shutdown(SocketShutdown.Both);
                    client_socket.Close();
                }
                catch
                {
                }
                lock (p_all_treads_data_lock)
                {
                    worker_data.remove_worker(p_thread_id);
                }
                Thread.CurrentThread.Abort();
                return;
            }

            int last_message_sent_id = 0;
            bool cancelled = false;

            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Context = new StreamingContext(StreamingContextStates.All);
            SerializationBinder serialization_binder = formatter.Binder;
            MemoryStream stream = new MemoryStream();
            byte[] data_buffer = new byte[1];

            status = ServerWorkerStatus.Satrting;

            Queue<byte[]> temp_receive_thread_queue = new Queue<byte[]>();

            WorkerReceiveThreadConstruct receive_thread_construct = new WorkerReceiveThreadConstruct(temp_receive_thread_queue);

            Thread receiver_thread = new Thread(() => ReceiveThread(receive_thread_construct, ref client_socket
                , ref timer, worker_data.worker_pulse_object));

            Queue<KeyValuePair<TypeOfSend, byte[]>> send_thread_queue = new Queue<KeyValuePair<TypeOfSend, byte[]>>();
            bool send_thread_queue_flag = false;
            bool send_thread_cancel_flag = false;
            object send_thread_pulse_object = new object();
            bool send_thread_receipt_received_flag = false;

            Thread send_thread = new Thread(() => SendThread(send_thread_queue, ref send_thread_queue_flag, ref send_thread_cancel_flag
                , ref send_thread_receipt_received_flag, send_thread_pulse_object, worker_data.worker_pulse_object, receive_thread_construct, client_socket, timer));

            receiver_thread.Start();
            send_thread.Start();
            timer.StartAndReset();

            status = ServerWorkerStatus.Normal;

            while (true)
            {
                lock (worker_data.worker_pulse_object)
                {
                    if (!worker_data.cancel_construct && !receive_thread_construct.receive_thread_queue_flag && !worker_data.send_to_worker_construct.send_to_worker_queue_flag)
                        Monitor.Wait(worker_data.worker_pulse_object);
                    if (worker_data.cancel_construct)
                    {
                        timer.PrimerStop();  
                        cancelled = true;
                        break;
                    }
                    if (receive_thread_construct.receive_thread_queue_flag)
                    {
                        if (receive_thread_construct.receive_thread_queue.Count > 0)
                        {
                            byte[] byte_received_data = receive_thread_construct.receive_thread_queue.Dequeue();
                            if (receive_thread_construct.receive_thread_queue.Count == 0)
                            {
                                receive_thread_construct.receive_thread_queue_flag = false;
                            }
                            if (byte_received_data.Length == 0)
                            {
                                timer.PrimerStop();
                                lock (worker_data.server_pulse_object)
                                {
                                    worker_data.receive_from_worker_construct.server_receive_quque.Enqueue(new MessageFromServerWorkerQueueObject(p_thread_id, new MessageFromServerWorker(TypeOfMessageFromServerWorker.ofllienRequestMessage, null)));
                                    worker_data.receive_from_worker_construct.server_receive_queue_flag = true;
                                    Monitor.Pulse(worker_data.server_pulse_object);
                                    cancelled = true;
                                    break;
                                }
                            }
                            else
                            {
                                stream = new MemoryStream(byte_received_data);

                                DataForSend data_received = null;
                                try
                                {
                                    data_received = (DataForSend)formatter.Deserialize(stream);
                                }
                                catch (Exception)
                                {
                                    stream = new MemoryStream();
                                    continue;
                                }
                                stream = new MemoryStream();
                                if (data_received.Get_data_for_send_type == TypeOfDataForSend.QuickAnswer && status == ServerWorkerStatus.WaitForAReceipt)
                                {
                                    QuickAnswer received_quick_answer = null;
                                    try
                                    {
                                        received_quick_answer = ((QuickAnswer)data_received.Get_message_object);
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                    if (received_quick_answer.Get_id == last_message_sent_id)
                                    {
                                        lock (send_thread_pulse_object)
                                        {
                                            send_thread_receipt_received_flag = true;
                                            Monitor.Pulse(send_thread_pulse_object);
                                            status = ServerWorkerStatus.Normal;
                                        }
                                    }
                                }
                                else if (data_received.Get_data_for_send_type == TypeOfDataForSend.QuickCheck)
                                {
                                    QuickCheck received_quick_check = null;
                                    try
                                    {
                                        received_quick_check = ((QuickCheck)data_received.Get_message_object);
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                    QuickAnswer quick_answer = new QuickAnswer(received_quick_check.Get_id);
                                    DataForSend quick_answer_for_send = new DataForSend(TypeOfDataForSend.QuickAnswer, quick_answer);  
                                    formatter.Serialize(stream, quick_answer_for_send);  
                                    byte[] byte_quick_answer = stream.GetBuffer();
                                    stream = new MemoryStream();
                                    lock (send_thread_pulse_object)  
                                    {
                                        send_thread_queue.Enqueue(new KeyValuePair<TypeOfSend, byte[]>(TypeOfSend.WithoutReceipt, byte_quick_answer));
                                        send_thread_queue_flag = true;
                                        Monitor.Pulse(send_thread_pulse_object);
                                    }
                                }
                                else if (data_received.Get_data_for_send_type == TypeOfDataForSend.UserMessage)
                                {
                                    UserMessageToServer user_message = null;
                                    try
                                    {
                                        user_message = ((UserMessageToServer)data_received.Get_message_object);  
                                    }
                                    catch
                                    {
                                        continue;
                                    }

                                    FinalMessageForServer final_message_for_server = null;
                                    try
                                    {
                                        final_message_for_server = user_message.Get_user_message;
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                    MessageFromServerWorkerQueueObject server_object = new MessageFromServerWorkerQueueObject(p_thread_id,
                                        new MessageFromServerWorker(TypeOfMessageFromServerWorker.FinalMessageForServer, final_message_for_server));
                                    lock (worker_data.server_pulse_object)
                                    {
                                        worker_data.receive_from_worker_construct.server_receive_quque.Enqueue(server_object);
                                        worker_data.receive_from_worker_construct.server_receive_queue_flag = true;
                                        Monitor.Pulse(worker_data.server_pulse_object);
                                    }

                                    QuickAnswer user_message_answer = new QuickAnswer(user_message.Get_id);
                                    DataForSend user_answer_for_send = new DataForSend(TypeOfDataForSend.QuickAnswer, user_message_answer);  
                                    formatter.Serialize(stream, user_answer_for_send);  
                                    lock (send_thread_pulse_object)
                                    {
                                        send_thread_queue.Enqueue(new KeyValuePair<TypeOfSend, byte[]>(TypeOfSend.WithoutReceipt, stream.GetBuffer()));
                                        send_thread_queue_flag = true;
                                        Monitor.Pulse(send_thread_pulse_object);
                                    }

                                    stream = new MemoryStream();
                                }
                            }

                        }
                        else
                        {
                            receive_thread_construct.receive_thread_queue_flag = false;
                        }
                    }
                    if (worker_data.send_to_worker_construct.send_to_worker_queue_flag)
                    {
                        if (status == ServerWorkerStatus.Normal)
                        {
                            if (worker_data.send_to_worker_construct.send_to_worker_quque.Count > 0)
                            {
                                MessageToServerWorker object_to_send = worker_data.send_to_worker_construct.send_to_worker_quque.Dequeue();
                                if (worker_data.send_to_worker_construct.send_to_worker_quque.Count == 0)
                                {
                                    worker_data.send_to_worker_construct.send_to_worker_queue_flag = false;
                                }
                                if (object_to_send.Get_type_of_message_to_serverworker == TypeOfMessageToServerWorker.QuickCheckRequest)
                                {
                                    QuickCheck quick_check_to_send = new QuickCheck(HelperFunctions.GetGUID());
                                    DataForSend client_data = new DataForSend(TypeOfDataForSend.QuickCheck, quick_check_to_send);
                                    formatter.Serialize(stream, client_data);
                                    byte[] byte_client_data = stream.GetBuffer();
                                    lock (send_thread_pulse_object)
                                    {
                                        send_thread_queue.Enqueue(new KeyValuePair<TypeOfSend, byte[]>(TypeOfSend.WithReceipt, byte_client_data));
                                        send_thread_queue_flag = true;
                                        Monitor.Pulse(send_thread_pulse_object);
                                    }
                                    last_message_sent_id = quick_check_to_send.Get_id;
                                    status = ServerWorkerStatus.WaitForAReceipt;
                                    stream = new MemoryStream();
                                }
                                else if (object_to_send.Get_type_of_message_to_serverworker == TypeOfMessageToServerWorker.FinalMessageToClient)
                                {
                                    FinalMessageForClient final_message = (FinalMessageForClient)object_to_send.Get_message_to_server_worker_object;
                                    UserMessageToClient user_message_to_send = new UserMessageToClient(HelperFunctions.GetGUID(), final_message);
                                    DataForSend client_data = new DataForSend(TypeOfDataForSend.UserMessage, user_message_to_send);
                                    formatter.Serialize(stream, client_data);
                                    byte[] byte_client_data = stream.GetBuffer();
                                    lock (send_thread_pulse_object)
                                    {
                                        send_thread_queue.Enqueue(new KeyValuePair<TypeOfSend, byte[]>(TypeOfSend.WithReceipt, byte_client_data));
                                        send_thread_queue_flag = true;
                                        Monitor.Pulse(send_thread_pulse_object);
                                    }
                                    last_message_sent_id = user_message_to_send.Get_id;
                                    status = ServerWorkerStatus.WaitForAReceipt;
                                    stream = new MemoryStream();
                                }

                            }
                            else
                            {
                                worker_data.send_to_worker_construct.send_to_worker_queue_flag = false;
                            }
                        }
                    }
                }
            }
            if (cancelled)
            {
                timer.PrimerStop();

                try
                {
                    receiver_thread.Abort(); 

                }
                catch
                {
                }
                try
                {
                    client_socket.Shutdown(SocketShutdown.Both);
                    client_socket.Close();
                }
                catch
                {
                }
                receiver_thread.Join(); 

                lock (send_thread_pulse_object)
                {
                    send_thread_cancel_flag = true;
                    Monitor.Pulse(send_thread_pulse_object);
                }
                send_thread.Join();   

                lock (p_all_treads_data_lock)
                {
                    worker_data.remove_worker(p_thread_id);
                }
                Thread.CurrentThread.Abort();
                return;
            }

        }

        static void listener_timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e, out bool Elapsed)
        {
            Elapsed = true;
        }

        private static void worker_timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e, SendToWorkerConstruct p_send_to_worker, object p_worker_pulse_object)
        {
            Thread quick_check_thread = new Thread(() => QuickCheckThread(p_send_to_worker, p_worker_pulse_object));
            quick_check_thread.Start();
        }
        private static void QuickCheckThread(SendToWorkerConstruct p_send_to_worker_construct, object p_worker_pulse_object)
        {
            lock (p_worker_pulse_object)
            {
                p_send_to_worker_construct.send_to_worker_quque.Enqueue(new MessageToServerWorker(TypeOfMessageToServerWorker.QuickCheckRequest, null));
                p_send_to_worker_construct.send_to_worker_queue_flag = true;
                Monitor.Pulse(p_worker_pulse_object);
            }
        }

        private static void ReceiveThread(WorkerReceiveThreadConstruct p_recieve_thread_construct, ref Socket p_client_socket
            , ref WorkerTimer p_worker_timer, object p_worker_pulse_object)
        {
            byte[] data_received = new byte[1024];
            while (true)
            {
                data_received = ReceiveData(p_client_socket);
                lock (p_worker_pulse_object)
                {
                    p_recieve_thread_construct.receive_thread_queue_flag = true;
                    p_recieve_thread_construct.receive_thread_queue.Enqueue(data_received);
                    p_worker_timer.StartAndReset();
                    Monitor.Pulse(p_worker_pulse_object);
                    if (data_received.Length == 0)
                    {
                        Thread.CurrentThread.Abort();
                        return;
                    }
                }
            }

        }
        private static byte[] ReceiveData(Socket p_client_socket)
        {
            byte[] data_size = new byte[4];
            int recv = 0;
            int h = 0;
            while (h < 4)
            {
                try
                {
                    recv = p_client_socket.Receive(data_size, 0, 4, SocketFlags.None);
                    h = 0;
                    break;
                }
                catch (SocketException exception)
                {
                    if (exception.ErrorCode == 10054) 
                    {
                        h = 0;
                        return new byte[0];
                    }
                    else
                    {
                        Thread.Sleep(100);
                        h++;
                        continue;
                    }
                }
            }
            if (h > 3)
            {
                return new byte[0];
            }
            if (recv == 0)
            {
                return new byte[0];
            }

            int size = BitConverter.ToInt32(data_size, 0);
            int data_received = 0;
            int data_left = size;
            int q_recieved = 0;
            byte[] main_data = new byte[size];
            while (data_received < size)
            {
                int k = 0;
                while (k < 4)
                {
                    try
                    {
                        q_recieved = p_client_socket.Receive(main_data, data_received, data_left, SocketFlags.None);
                        k = 0;
                        break;
                    }
                    catch (SocketException exception)
                    {
                        if (exception.ErrorCode == 10054)
                        {
                            k = 0;
                            return new byte[0]; 
                        }
                        else
                        {
                            Thread.Sleep(100);
                            k++;
                            continue;
                        }
                    }
                }
                if (k > 3)
                {
                    return new byte[0];
                }
                if (q_recieved == 0)
                {
                    return new byte[0];
                }

                data_received += q_recieved;
                data_left -= q_recieved;
            }
            return main_data;
        }

        private static void SendThread(Queue<KeyValuePair<TypeOfSend, byte[]>> p_send_thread_queue, ref bool p_send_thread_queue_flag
            , ref bool p_send_thread_cancel_flag, ref bool p_receipt_received_flag, object p_send_thread_pulse_object
            , object p_worker_pulse_object, WorkerReceiveThreadConstruct p_recieve_thread_construct, Socket p_client_socket
            , WorkerTimer p_worker_timer)
        {
            SendWorkerThreadStatus status = SendWorkerThreadStatus.Normal;
            System.Timers.Timer send_timer = new System.Timers.Timer();
            send_timer.Interval = 3000;
            send_timer.AutoReset = false;

            Dictionary<int, Thread> all_mini_threads = new Dictionary<int, Thread>();
            object all_mini_threads_lock = new object();

            bool cancelled = false;
            int retry_counts = 0;
            byte[] last_message_sent = new byte[1];

            SendThreadResendConstruct resend_construct = new SendThreadResendConstruct();
            send_timer.Elapsed += new System.Timers.ElapsedEventHandler((sender, e) => send_timer_Elapsed(sender, e, resend_construct, p_send_thread_pulse_object));

            while (true)
            {
                lock (p_send_thread_pulse_object)
                {
                    if (!p_send_thread_cancel_flag && !p_receipt_received_flag && !p_send_thread_queue_flag && !resend_construct.resend_flag)
                    {
                        Monitor.Wait(p_send_thread_pulse_object);
                    }
                    if (p_send_thread_cancel_flag)
                    {
                        status = SendWorkerThreadStatus.End;
                        send_timer.Stop(); 
                        cancelled = true;
                        break;
                    }
                    if (p_receipt_received_flag && status == SendWorkerThreadStatus.WaitingForAReceipt)
                    {
                        send_timer.Stop();
                        retry_counts = 0;
                        status = SendWorkerThreadStatus.Normal;
                        resend_construct.resend_flag = false;
                        p_receipt_received_flag = false;
                    }
                    if (resend_construct.resend_flag == true && status == SendWorkerThreadStatus.WaitingForAReceipt)
                    {
                        retry_counts++;
                        resend_construct.resend_flag = false; 
                        if (retry_counts < 3)
                        {
                            send_timer.Stop();
                            send_timer.Start();
                            int sent_data_length = SendData(p_client_socket, last_message_sent);
                            if (sent_data_length > 0)
                            {
                                p_worker_timer.StartAndReset();
                            }
                        }
                        else
                        {
                            send_timer.Stop();
                            int mini_thread_id = HelperFunctions.GetGUID();
                            p_worker_timer.StartAndReset();
                            status = SendWorkerThreadStatus.Suspended;
                            Thread send_cancel_bytes_to_worker_thead = new Thread(() => SendCancelBytesToWorker(p_worker_pulse_object, p_recieve_thread_construct
                                ,all_mini_threads,all_mini_threads_lock,mini_thread_id));
                            lock (all_mini_threads_lock)
                            {
                                all_mini_threads.Add(mini_thread_id, send_cancel_bytes_to_worker_thead);
                            }
                            send_cancel_bytes_to_worker_thead.Start();
                        }
                    }
                    if (p_send_thread_queue_flag && (status == SendWorkerThreadStatus.Normal || status == SendWorkerThreadStatus.WaitingForAReceipt))
                    {
                        if (p_send_thread_queue.Count > 0)
                        {
                            if ((p_send_thread_queue.ElementAt(0).Key == TypeOfSend.WithReceipt && status == SendWorkerThreadStatus.Normal)
                                || (p_send_thread_queue.ElementAt(0).Key == TypeOfSend.WithoutReceipt && (status == SendWorkerThreadStatus.Normal
                                || status == SendWorkerThreadStatus.WaitingForAReceipt)))
                            {
                                KeyValuePair<TypeOfSend, byte[]> send_object = p_send_thread_queue.Dequeue();
                                if (p_send_thread_queue.Count == 0)
                                {
                                    p_send_thread_queue_flag = false;
                                }

                                if (send_object.Key == TypeOfSend.WithReceipt && status == SendWorkerThreadStatus.Normal)
                                {
                                    status = SendWorkerThreadStatus.WaitingForAReceipt;
                                    last_message_sent = send_object.Value;
                                    int sent_data_length = SendData(p_client_socket, send_object.Value);
                                    if (sent_data_length > 0)
                                    {
                                        p_worker_timer.StartAndReset(); 
                                    }
                                    send_timer.Start();
                                }
                                else if (send_object.Key == TypeOfSend.WithoutReceipt)
                                {
                                    SendData(p_client_socket, send_object.Value);
                                    p_worker_timer.StartAndReset();
                                }
                            }

                        }
                    }
                    else
                    {
                        p_send_thread_queue_flag = false;
                    }
                }
            }
            if (cancelled)
            {
                send_timer.Stop();
                Thread.CurrentThread.Abort();
            }
        }

        private static void SendCancelBytesToWorker(object p_worker_pulse_object, WorkerReceiveThreadConstruct p_recieve_thread_construct
            ,Dictionary<int, Thread> p_all_mini_threads ,object p_all_mini_threads_lock ,int p_mini_thread_id )
        {
            lock (p_worker_pulse_object)
            {
                p_recieve_thread_construct.receive_thread_queue.Enqueue(new byte[0]);
                p_recieve_thread_construct.receive_thread_queue_flag = true;
                Monitor.Pulse(p_worker_pulse_object);
            }
            lock (p_all_mini_threads_lock)
            {
                if(p_all_mini_threads.ContainsKey(p_mini_thread_id))
                {
                    p_all_mini_threads.Remove(p_mini_thread_id);
                }
            }
        }

        private static void send_timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e, SendThreadResendConstruct p_resend_construct
            , object p_send_thread_pulse_object)
        {
            Thread resend_thread = new Thread(() => ResendThread(p_resend_construct, p_send_thread_pulse_object));
            resend_thread.Start();
        }
        private static void ResendThread(SendThreadResendConstruct p_resend_construct, object p_send_thread_pulse_object)
        {
            lock (p_send_thread_pulse_object)
            {
                p_resend_construct.resend_flag = true;
                Monitor.Pulse(p_send_thread_pulse_object);
            }
        }
        private static int SendData(Socket p_client_socket, byte[] p_data)
        {
            byte[] data_size = new byte[4];
            data_size = BitConverter.GetBytes(p_data.Length);
            byte[] final = new byte[4 + p_data.Length];

            data_size.CopyTo(final, 0);
            p_data.CopyTo(final, 4);

            int size = final.Length;
            int data_sent = 0;
            int q_sent_data = 0;
            int data_left = final.Length;

            while (data_sent < size)
            {
                int j = 0;
                while (j < 4)
                {
                    try
                    {
                        q_sent_data = p_client_socket.Send(final, data_sent, data_left, SocketFlags.None);
                        j = 0;
                        break;
                    }
                    catch (SocketException exception)
                    {
                        if (exception.ErrorCode == 10054)
                        {
                            return 0;
                        }
                        else
                        {
                            Thread.Sleep(100);
                            j++;
                            continue;
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                        return 0;
                    }
                }
                if (j > 3)
                {
                    return 0;
                }
                data_sent += q_sent_data;
                data_left -= q_sent_data;
            }
            return data_sent;
        }


    }

    public class WorkerReceiveThreadConstruct
    {
        public Queue<byte[]> receive_thread_queue;
        public bool receive_thread_queue_flag;

        public WorkerReceiveThreadConstruct(Queue<byte[]> p_receive_thread_queue)
        {
            receive_thread_queue = p_receive_thread_queue;
            receive_thread_queue_flag = false;
        }
    }

    public class SendThreadResendConstruct
    {
        public bool resend_flag;

        public SendThreadResendConstruct()
        {
            resend_flag = false;
        }
    }

    public enum SendWorkerThreadStatus
    {
        Normal,
        WaitingForAReceipt,
        Suspended,
        End,
    }

    public enum TypeOfSend
    {
        WithReceipt,
        WithoutReceipt
    }

    public enum ServerWorkerStatus
    {
        Satrting,
        WaithForAclinet,
        Normal,
        WaitForAReceipt
    }

    public class WorkerTimer
    {
        System.Timers.Timer timer;
        bool primer_stop;
        ReaderWriterLockSlim timer_lock;

        public WorkerTimer(System.Timers.Timer p_timer)
        {
            timer = p_timer;
            primer_stop = false;
            timer_lock = new ReaderWriterLockSlim();
        }

        public void StartAndReset()
        {
            timer_lock.EnterWriteLock();
            if (!primer_stop)
            {
                timer.Stop();
                timer.Start();
            }
            timer_lock.ExitWriteLock();
        }

        public void PrimerStop()
        {
            timer_lock.EnterWriteLock();
            primer_stop = true;
            timer.Stop();
            timer_lock.ExitWriteLock();
        }
    }


}

