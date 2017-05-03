using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.Net.Sockets;

namespace ServerTcpChat.Classes
{

    static class OtherThreads
    {
        public static void WorkerProducerThread(Dictionary<int, KeyValuePair<Thread, ServerWorkerData>> p_all_workers_data, object p_all_workers_data_lock
            , object p_producer_pulse_object, ReceiveFromServerWorkerConstruct p_receive_from_worker_construct, WorkersPortNumberConstruct p_workers_port_number_construct
            , object p_server_pulse_object, IPAddress p_server_ip_address)
        {

            while (true)
            {
                lock (p_producer_pulse_object)
                {
                    if (!p_workers_port_number_construct.workers_port_number_queue_flag)
                        Monitor.Wait(p_producer_pulse_object);

                    if (p_workers_port_number_construct.workers_port_number_queue_flag)
                    {
                        if (p_workers_port_number_construct.workers_port_number_queue.Count > 0)
                        {
                            int port_num = p_workers_port_number_construct.workers_port_number_queue.Dequeue();
                            if (p_workers_port_number_construct.workers_port_number_queue.Count == 0)
                                p_workers_port_number_construct.workers_port_number_queue_flag = false;
                            ServerWorkerData new_worker_data = new ServerWorkerData(p_receive_from_worker_construct, p_server_pulse_object, new RemoveWorker(p_all_workers_data.Remove));
                            int new_thread_id = HelperFunctions.GetGUID();
                            Thread new_worker_thread = new Thread(() => WorkerThread.WorkerMainThread(new_worker_data, port_num, new_thread_id, p_all_workers_data_lock, p_server_ip_address));
                            lock (p_all_workers_data_lock)
                            {
                                p_all_workers_data.Add(new_thread_id, new KeyValuePair<Thread, ServerWorkerData>(new_worker_thread, new_worker_data));
                            }
                            new_worker_thread.Start();  
                        }
                        else
                        {
                            p_workers_port_number_construct.workers_port_number_queue_flag = false;
                        }
                    }
                }
            }
        }

        public static void DistributerThread(SendToDistributerConstruct p_send_to_disributer_construct, object p_distributer_pulse_object
            , Dictionary<int, KeyValuePair<Thread, ServerWorkerData>> p_all_workers_data, object p_all_workers_data_lock)
        {
            Dictionary<int, Thread> all_mini_threads = new Dictionary<int, Thread>();
            object all_mini_threads_lock = new object();
            while (true)
            {
                lock (p_distributer_pulse_object)
                {
                    if (!p_send_to_disributer_construct.send_to_distribuer_queue_flag)
                        Monitor.Wait(p_distributer_pulse_object);

                    if (p_send_to_disributer_construct.send_to_distribuer_queue_flag)
                    {
                        if (p_send_to_disributer_construct.send_to_distributer_queue.Count > 0)
                        {
                            MessageToDistributer object_to_do = p_send_to_disributer_construct.send_to_distributer_queue.Dequeue();
                            if (object_to_do.Get_type_of_message_to_distributer == TypeOfMessageToDistributer.CancelAServerWorkerRequest)
                            {
                                int thread_id_to_cancel = object_to_do.Get_thread_id;
                                int mini_thread_id = HelperFunctions.GetGUID();

                                Thread cancelling_thread = new Thread(() => CancelAServerWorkerThread(p_all_workers_data, p_all_workers_data_lock, thread_id_to_cancel
                                    , all_mini_threads_lock, all_mini_threads, mini_thread_id));

                                lock (all_mini_threads_lock)
                                {
                                    all_mini_threads.Add(mini_thread_id, cancelling_thread);
                                }
                                cancelling_thread.Start();
                            }

                            else if (object_to_do.Get_type_of_message_to_distributer == TypeOfMessageToDistributer.MessageToServerWorker)
                            {
                                int thread_id_to_get_message = object_to_do.Get_thread_id;
                                int mini_thread_id = HelperFunctions.GetGUID();

                                Thread transport_thread = new Thread(() => TransferAMessageToAServerWorkerThread(object_to_do, thread_id_to_get_message, p_all_workers_data_lock,
                                    p_all_workers_data, all_mini_threads_lock, all_mini_threads, mini_thread_id));

                                lock (all_mini_threads_lock)
                                {
                                    all_mini_threads.Add(mini_thread_id, transport_thread);
                                }
                                transport_thread.Start();
                            }
                        }
                        else
                        {
                            p_send_to_disributer_construct.send_to_distribuer_queue_flag = false;
                        }
                    }
                }
            }
        }
        private static void CancelAServerWorkerThread(Dictionary<int, KeyValuePair<Thread, ServerWorkerData>> p_all_workers_data
            , object p_all_workers_data_lock, int p_thread_id, object p_all_mini_threads_lock, Dictionary<int, Thread> p_all_mini_threads
            , int p_mini_thread_id)
        {
            lock (p_all_workers_data_lock)
            {
                if (p_all_workers_data.ContainsKey(p_thread_id))
                {
                    lock (p_all_workers_data[p_thread_id].Value.worker_pulse_object)
                    {
                        p_all_workers_data[p_thread_id].Value.cancel_construct = true;
                        Monitor.Pulse(p_all_workers_data[p_thread_id].Value.worker_pulse_object);
                    }
                }
            }
            lock (p_all_mini_threads_lock)
            {
                if (p_all_mini_threads.ContainsKey(p_mini_thread_id))
                {
                    p_all_mini_threads.Remove(p_mini_thread_id);
                }
            }
            Thread.CurrentThread.Abort();
            return;
        }
        private static void TransferAMessageToAServerWorkerThread(MessageToDistributer p_message_to_transfer, int p_thread_id, object p_all_workers_data_lock
            , Dictionary<int, KeyValuePair<Thread, ServerWorkerData>> p_all_workers_data, object p_all_mini_threads_lock, Dictionary<int, Thread> p_all_mini_threads
            , int p_mini_thread_id)
        {
            lock (p_all_workers_data_lock)
            {
                if (p_all_workers_data.ContainsKey(p_thread_id))
                {
                    lock (p_all_workers_data[p_thread_id].Value.worker_pulse_object)
                    {
                        p_all_workers_data[p_thread_id].Value.send_to_worker_construct.send_to_worker_quque.Enqueue(p_message_to_transfer.Get_message_to_server_worker);
                        p_all_workers_data[p_thread_id].Value.send_to_worker_construct.send_to_worker_queue_flag = true;
                        Monitor.Pulse(p_all_workers_data[p_thread_id].Value.worker_pulse_object);
                    }
                }
            }
            lock (p_all_mini_threads_lock)
            {
                if (p_all_mini_threads.ContainsKey(p_mini_thread_id))
                {
                    p_all_mini_threads.Remove(p_mini_thread_id);
                }
            }
            Thread.CurrentThread.Abort();
            return;
        }
        public static void UDPThread(WorkersPortNumberConstruct p_workers_port_number_construct, object p_worker_producer_pulse_object, int p_check_data
            , IPEndPoint p_server_udp_ip_endpoint, IPAddress p_server_ipd_address)
        {
            Socket udp_socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            int k = 0;
            while (k < 4)
            {
                try
                {
                    udp_socket.Bind(p_server_udp_ip_endpoint);
                    k = 0;
                    break;
                }
                catch (SocketException)
                {
                    k++;
                    continue;
                }
            }
            if (k > 3)
            {
                Console.WriteLine("Error in binding udp ipenpoint");
                try
                {
                    Environment.Exit(1);
                }
                catch
                {
                }
                Thread.CurrentThread.Abort();
                return;
            }

            byte[] buffer = new byte[32];
            int h = 0;
            int l = 0;

            List<int> socket_stoping_app_errors = GetAppKillingErrorsCodes();

            while (true)
            {
                EndPoint client_endpoint = (EndPoint)p_server_udp_ip_endpoint;
                try
                {
                    buffer = new byte[32];
                    udp_socket.ReceiveFrom(buffer, ref client_endpoint);
                    int received_int = BitConverter.ToInt32(buffer, 0);
                    if (p_check_data != received_int)
                    {
                        h = 0;
                        l = 0;
                        continue;
                    }
                    l = 0;
                    h = 0;
                }
                catch (SocketException exception)
                {
                    int socket_exception_error_code = exception.ErrorCode;
                    if (socket_stoping_app_errors.Contains(socket_exception_error_code))
                    {
                        h++;
                        Console.WriteLine("network error with code:" + exception.ErrorCode.ToString() + "\n" + exception.Message);
                        Thread.Sleep(50);
                        if (h > 5)
                        {
                            Console.WriteLine("Too Much Socket Error Occured. Killing Application");
                            try
                            {
                                Environment.Exit(2);
                            }
                            catch
                            {
                            }
                            Thread.CurrentThread.Abort();
                            return;
                        }
                    }
                    else
                    {
                        h = 0;
                    }
                }
                catch (System.Security.SecurityException exception)
                {
                    Console.WriteLine("Security Error: " + exception.Message);
                    l++;
                    if (l > 5)
                    {
                        Console.WriteLine("Too Much Security Error Occured. Killing Application. Restart Program");
                        try
                        {
                            Environment.Exit(1);
                        }
                        catch
                        {
                        }
                        Thread.CurrentThread.Abort();
                        return;
                    }
                }
                catch (Exception)
                {
                    continue;
                }

                int g = 0;
                int random_port = 0;
                while (g < 4)
                {
                    try
                    {
                        TcpListener temp_listener = new TcpListener(p_server_ipd_address, 0);
                        temp_listener.Start();
                        random_port = ((IPEndPoint)temp_listener.LocalEndpoint).Port;
                        temp_listener.Stop();
                        g = 0;
                        break;
                    }
                    catch
                    {
                        g++;
                        continue;
                    }
                }
                if (g > 3)
                {
                    Console.WriteLine("Error in all0cating tcp port to client. please restart the application.");
                    try
                    {
                        Environment.Exit(4);
                    }
                    catch
                    {
                    }
                    Thread.CurrentThread.Abort();
                    return;
                }

                try
                {
                    udp_socket.SendTo(BitConverter.GetBytes(random_port), client_endpoint);
                }
                catch (SocketException exception)
                {
                    continue;
                }
                lock (p_worker_producer_pulse_object)
                {
                    p_workers_port_number_construct.workers_port_number_queue.Enqueue(random_port);
                    p_workers_port_number_construct.workers_port_number_queue_flag = true;
                    Monitor.Pulse(p_worker_producer_pulse_object);
                }



            }

        }

        private static List<int> GetAppKillingErrorsCodes()
        {
            List<int> socket_stoping_app_errors = new List<int>();
            socket_stoping_app_errors.Add(10050);
            socket_stoping_app_errors.Add(10051);
            socket_stoping_app_errors.Add(10056);
            socket_stoping_app_errors.Add(10058);
            socket_stoping_app_errors.Add(10060);
            socket_stoping_app_errors.Add(10061);
            socket_stoping_app_errors.Add(10038);
            socket_stoping_app_errors.Add(10041);
            socket_stoping_app_errors.Add(10042);
            socket_stoping_app_errors.Add(10043);
            socket_stoping_app_errors.Add(10044);
            socket_stoping_app_errors.Add(10045);
            socket_stoping_app_errors.Add(10048);
            return socket_stoping_app_errors;
        }

    }

}
