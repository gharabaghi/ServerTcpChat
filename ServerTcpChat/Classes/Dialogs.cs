using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerTcpChat.Classes;
using System.Data.SqlClient;
using CommonChatTypes;

namespace ServerTcpChat.Classes
{

    public delegate Se_BaseBooleanFunctionResult OnlineAreFriends(string p_first_person_user_name, string p_second_person_user_name);
    public delegate void GetAgreementAnswer(string p_user_name, int p_agreement_id, bool p_answer, TypeOfAgreement p_agreement_type);
    public delegate Se_BaseBooleanFunctionResult Login(int p_thread_id, string p_user_name, string p_password);
    public delegate void AuthSend(string p_user_name, DialogMessageForClient p_message);
    public delegate void UnAuthSend(int p_thread_id, DialogMessageForClient p_message);
    public delegate bool IsLoggedIn(string p_user_name);
    public delegate List<string> GetFriendsList(string p_user_name);
    public delegate UserStatus GetUserStatus(string p_user_name);
    public delegate bool IstherUser(string p_user_name); 
    public delegate Se_BaseBooleanFunctionResult SignUp(string p_user_name, string p_password);
    public delegate bool IsThereUnauthWorkerThread(int p_unauththread_id);
    public delegate void CreateOfflineMessage(string p_user_name, OfflineMessage p_message);
    public delegate List<OfflineMessage> GetOfflineMessages(string p_user_name);
    public delegate List<AgreementInvitationInfo> GetAllAgreementInvitation(string p_user_name);
    public delegate void StartSendToClinetFormalMessage(string p_receiver_user_name, string p_sender_user_name, string p_message_text, int p_message_id);

    public delegate List<int> GetPublicChatIds();
    public delegate Se_BaseBooleanFunctionResult JoinPublicChatRequest(string p_user_name, int p_chat_id);
    public delegate void ServerInformChatLeave(string p_user_name, int p_chat_id, TypeOfChat p_chat_type);
    public delegate Se_BaseIntFunctionResult CreatePrivateChat(string p_first_person_user_name, string p_second_person_user_name);
    public delegate bool IsTherePrivateChat(int p_private_chat_id);
    public delegate bool IsUserInPublicChat(string p_user_name, int p_public_chat_id);
    public delegate List<string> GetPublicChatUsersList(int p_chat_id);
    public delegate void CreatePublicChat(int p_chat_id, int p_max_users_count);
    public delegate Se_BaseBooleanFunctionResult GetPrivateChatInvitationAnswer(string p_user_name, int p_chat_id, bool p_answer);

    public delegate Se_BaseBooleanFunctionResult CreateAddAgreement(string p_starter_user_name, string p_invited_user_id);

    public delegate void CreateFormalMessageRequest(FormalMessage p_message, string p_receiver_user_name);
    public delegate void StartClientCreatedPrivateChatInform(string p_user_name, string p_starter_user_name, int p_chat_id);
    public delegate void OfflineMessagesReadInform(string p_user_name, List<int> p_message_ids);
    public delegate AgreementInvitationInfo GetAUserAgreementInvitation(string p_user_name, int p_agreement_id);
    public delegate void StartClientInvitedAgreementInform(string p_user_name, int p_agreement_id);

    public delegate void ALevelOfDialog();
    public delegate void Remove(int p_dialog_id);

    public delegate void LoginRequestSucc();
    public delegate void LoginRequestFail(string p_comment);
    public delegate void ShowMessageToUser(string Messsage);
    public delegate void LoginDataReceived(object p_login_data_object);
    public delegate void LoginDataNotReceived(string p_comment);
    public delegate void SendToServer(DialogMessageForServer p_message);

    public class DialogLevelsInformation
    {
        ALevelOfDialog level_function;
        DialogLevelType level_type;
        bool level_executed;
        public DialogLevelsInformation(ALevelOfDialog p_level_function, DialogLevelType p_level_type)
        {
            level_function = p_level_function;
            level_type = p_level_type;
            level_executed = false;
        }
        public ALevelOfDialog Get_level_function
        {
            get
            {
                return level_function;
            }
        }
        public DialogLevelType Get_level_type
        {
            get
            {
                return level_type;
            }
        }
        public bool Set_Get_level_executed
        {
            set
            {
                level_executed = value;
            }
            get
            {
                return level_executed;
            }
        }
    }

    public class ExceptionOccurenceStruct
    {
        bool exception_occured;
        public bool Get_exception_occured
        {
            get { return exception_occured; }
        }

        List<string> exception_message;
        public List<string> Get_exception_message
        {
            get { return exception_message; }
        }

        public void RegisterAnException(string p_exception_message)
        {
            exception_occured = true;
            exception_message.Add(p_exception_message);
        }

        public ExceptionOccurenceStruct()
        {
            exception_occured = false;
            exception_message = new List<string>();
        }
    }

    public abstract class BaseDialog
    {
        protected int dialog_id;
        protected int last_message_number;
        protected int current_level;
        protected int level_counts;
        protected DialogMessageForServer last_message_received;
        protected TypeOfDialog dialog_type;
        protected Dictionary<int, DialogLevelsInformation> all_dialog_levels;
        protected int last_message_sent_id;
        protected Remove remove_dialog_from_manager;
        protected List<DialogMessageForServer> cache;
        protected DialogStatus status;


        protected ExceptionOccurenceStruct exception_occurance;
        protected int retry_counts;
        protected int max_retry_count;

        public ExceptionOccurenceStruct Get_exception_occurance
        {
            get { return exception_occurance; }
        }
        public int Get_retry_counts
        {
            get { return retry_counts; }
        }
        public int Get_last_message_number
        {
            get { return last_message_number; }
        }
        public int Get_current_level
        {
            get { return current_level; }
        }
        public int Get_level_counts
        {
            get { return level_counts; }
        }
        public DialogMessageForServer Get_last_message_received
        {
            get { return last_message_received; }
        }
        public TypeOfDialog Get_dialog_type
        {
            get { return dialog_type; }
        }
        public Dictionary<int, DialogLevelsInformation> Get_all_dialog_levels
        {
            get { return all_dialog_levels; }
        }
        public int Get_last_message_sent_id
        {
            get { return last_message_sent_id; }
        }
        public List<DialogMessageForServer> Get_cache
        {
            get { return cache; }
        }
        public DialogStatus Get_status
        {
            get { return status; }
        }
        public int Get_dialog_id
        {
            get { return dialog_id; }
        }
        public int Get_max_retry_count
        {
            get { return max_retry_count; }
        }


        protected void BaseConstruct(int p_dialog_id, TypeOfDialog p_dialog_type, Remove p_remove_dialog_from_manager, int p_max_retry_counts)
        {
            dialog_type = p_dialog_type;
            dialog_id = p_dialog_id;
            remove_dialog_from_manager = p_remove_dialog_from_manager;

            current_level = 0;
            last_message_number = 0;
            last_message_received = null;
            all_dialog_levels = new Dictionary<int, DialogLevelsInformation>();
            cache = new List<DialogMessageForServer>();
            status = DialogStatus.Running;
            last_message_sent_id = 0;
            exception_occurance = new ExceptionOccurenceStruct();
            retry_counts = 0;
            max_retry_count = p_max_retry_counts;
        }

        protected void Start()
        {
            LevelAccept();
        }

        public void ReceiveMessage(DialogMessageForServer p_received_message)
        {
            ProcessMessage(p_received_message);
        }

        public void ManagerRemoveRequestPath()
        {
            CancelDialog("manager removed dialog");
            return;
        }

        protected void ProcessMessage(DialogMessageForServer p_message)
        {
            if (!HelperFunctions.DialogMessageObjectInvestigate(p_message))
            {
                exception_occurance.RegisterAnException("Received message object is  not valid");
                return;
            }
            if (p_message.Get_message_number_in_dialog == last_message_number + 1)
            {
                if (p_message.Get_message_object_type == TypeOfDialogMessage.CancelDialog)
                {
                    if (!(current_level == level_counts && all_dialog_levels[current_level].Set_Get_level_executed))
                    {
                        End();
                        return;
                    }
                    return;
                }

                if (status == DialogStatus.WaitingForAMessage || status == DialogStatus.WaitingForAReceipt)
                {

                    last_message_number = p_message.Get_message_number_in_dialog;
                    if (status == DialogStatus.WaitingForAMessage && p_message.Get_message_object_type != TypeOfDialogMessage.ReceiptMessage &&
                        Get_all_dialog_levels[current_level].Get_level_type == DialogLevelType.WaitingForMessageReceive)
                    {
                        status = DialogStatus.MessageInvestigation;
                        last_message_received = p_message;
                        all_dialog_levels[current_level].Get_level_function();
                    }
                    else if (status == DialogStatus.WaitingForAReceipt && p_message.Get_message_object_type == TypeOfDialogMessage.ReceiptMessage &&
                        all_dialog_levels[current_level].Get_level_type == DialogLevelType.SendingAMessage)
                    {
                        Di_Mess_ReceiptMessage temp_receipt = (Di_Mess_ReceiptMessage)p_message.Get_message_object;
                        ReceiptInvestigate(temp_receipt);
                    }
                }
            }

            else if (p_message.Get_message_number_in_dialog > last_message_number + 1 && p_message.Get_dialog_id == this.dialog_id)
            {
                cache.Add(p_message);
                return;
            }
        }

        protected void LevelAccept()
        {
            if (current_level == 0)
            {
                if (all_dialog_levels.Count != level_counts)
                {
                    exception_occurance.RegisterAnException("get_all_dialog_levels.Count != level_counts");
                    return;
                }
            }
            if (status != DialogStatus.ReceipptRejected && status != DialogStatus.MessageRejected)
            {
                retry_counts = 0;
            }

            if (status == DialogStatus.MessageInvestigation)
            {
                status = DialogStatus.MessageAccepted;
                AccMessage(last_message_received);
            }
            else if (status == DialogStatus.ReceiptAccepted)
            {

            }


            if (current_level != 0)
            {
                all_dialog_levels[current_level].Set_Get_level_executed = true;
                LevelAccActs();
            }

            if (current_level == level_counts)
            {
                all_dialog_levels[current_level].Set_Get_level_executed = true;
                End();
            }
            else if (all_dialog_levels[current_level + 1].Get_level_type == DialogLevelType.SendingAMessage)
            {
                status = DialogStatus.Running;
                current_level++;
                all_dialog_levels[current_level].Get_level_function();
            }
            else if (all_dialog_levels[current_level + 1].Get_level_type == DialogLevelType.WaitingForMessageReceive)
            {
                current_level++;
                status = DialogStatus.WaitingForAMessage;
                TryCache();
            }

            if (all_dialog_levels[current_level].Get_level_type == DialogLevelType.WaitingForMessageReceive)
                CheckCache();
        }
        protected void LevelReject(string p_reject_comment)
        {
            LevelRejActs();
            if (retry_counts > max_retry_count)
            {
                Send(TypeOfDialogMessage.CancelDialog, new Di_Mess_CancelDialogMessage("Retry counts became more than " + max_retry_count.ToString() + " times. server reason:" + p_reject_comment));
                End();
            }

            if (status == DialogStatus.MessageInvestigation)
            {
                status = DialogStatus.MessageRejected;
                retry_counts++;
                RejMessage(last_message_received, p_reject_comment);
                RollBack();
                return;
            }
            else if (status == DialogStatus.ReceipptRejected)
            {
                status = DialogStatus.ReceipptRejected;
                retry_counts++;
                RollBack();
                return;
            }

            return;
        }

        protected virtual void LevelAccActs()
        {
        }
        protected virtual void LevelRejActs()
        {
        }
        protected virtual void WaitingStateActs()
        {
        }

        protected virtual void RollBack()
        {
            if (current_level > 1)
            {
                all_dialog_levels[current_level - 1].Set_Get_level_executed = false;
            }
            current_level--;
            LevelAccept();
        }

        protected virtual void End()
        {
            if (current_level == level_counts && all_dialog_levels[current_level].Set_Get_level_executed)
                Result();
            else
                DialogRejectingActs();
            retry_counts = 0;
            status = DialogStatus.End;
            RemoveThisDialog();
        }

        protected void ReceiptInvestigate(Di_Mess_ReceiptMessage p_receipt_to_be_investigated)
        {
            status = DialogStatus.ReceiptInvestigation;
            if (p_receipt_to_be_investigated.Get_message_rec_status == ReceiptStatus.Accepted)
            {
                if (((Di_Mess_Rec_AcceptMessage)p_receipt_to_be_investigated.Get_rec_message).Get_accpepted_message_id == last_message_sent_id)
                {
                    status = DialogStatus.ReceiptAccepted;
                    LevelAccept();
                    return;
                }
                else
                {
                    exception_occurance.RegisterAnException("Receipt tells message has been accepted but id is not valid");
                    return;
                }
            }

            else if (p_receipt_to_be_investigated.Get_message_rec_status == ReceiptStatus.Rejected)
            {
                if (((Di_Mess_Rec_RejectMessage)p_receipt_to_be_investigated.Get_rec_message).Get_rejected_message_id == last_message_sent_id)
                {
                    status = DialogStatus.ReceipptRejected;
                    LevelReject(((Di_Mess_Rec_RejectMessage)p_receipt_to_be_investigated.Get_rec_message).Get_comment_for_rejecting);
                }
                else
                {
                    exception_occurance.RegisterAnException("Receipt tells message has been rejected but id is not valid");
                    return;
                }
            }
        }

        protected virtual void DialogRejectingActs()
        {
        }
        protected virtual void Result()
        {
        }

        protected void CheckCache()
        {
            for (int i = 0; i < cache.Count; i++)
            {
                if (cache[i].Get_message_number_in_dialog <= last_message_number)
                {
                    cache.RemoveAt(i);
                }
            }
        }
        protected virtual void TryCache()
        {
            int level_before_try = current_level;
            int last_message_number_before_try = last_message_number;
            DialogStatus status_before_try = status;
            for (int i = 0; i < cache.Count; i++)
            {
                if (level_before_try == current_level && last_message_number_before_try == last_message_number && status_before_try == status)
                {
                    DialogMessageForServer temp_message = cache[i];
                    cache.RemoveAt(i);
                    ProcessMessage(temp_message);
                }
                else
                {
                    return;
                }
            }

        }

        protected virtual void CancelDialog(string p_cancel_comment)
        {
            if (!(current_level == 1 && all_dialog_levels[1].Get_level_type == DialogLevelType.SendingAMessage && retry_counts == 0))
                Send(TypeOfDialogMessage.CancelDialog, new Di_Mess_CancelDialogMessage(p_cancel_comment));
            End();
            return;
        }

        protected void Send(TypeOfDialogMessage p_message_object_type, object p_message_object)
        {

            DialogMessageForClient dialog_message_to_be_sent = new DialogMessageForClient(HelperFunctions.GetGUID(), dialog_id, last_message_number + 1, dialog_type, p_message_object, p_message_object_type);
            if (!HelperFunctions.DialogMessageObjectInvestigate(dialog_message_to_be_sent))
            {
                exception_occurance.RegisterAnException("send message object is not valid");
                return;
            }

            SendFormalDialogMessage(dialog_message_to_be_sent);
            if (all_dialog_levels[current_level].Get_level_type == DialogLevelType.SendingAMessage && p_message_object_type != TypeOfDialogMessage.CancelDialog)
            {
                status = DialogStatus.WaitingForAReceipt;
                WaitingStateActs();
            }

            if (p_message_object_type == TypeOfDialogMessage.CancelDialog)
            {
                status = DialogStatus.Canceling;
            }
            last_message_number = dialog_message_to_be_sent.Get_message_number_in_dialog;
            last_message_sent_id = dialog_message_to_be_sent.Get_message_id;

        }
        protected void AccMessage(DialogMessageForServer p_message_to_accept)
        {
            status = DialogStatus.MessageAccepted;
            SendAccMessage(p_message_to_accept.Get_message_id);
        }
        protected void RejMessage(DialogMessageForServer p_message_to_reject, string p_rej_comment)
        {
            status = DialogStatus.ReceipptRejected;
            SendRejMessage(p_message_to_reject.Get_message_id, p_rej_comment);
        }
        protected abstract void SendFormalDialogMessage(DialogMessageForClient p_message_to_be_sent);
        protected abstract void SendAccMessage(int p_accepted_message_id);
        protected abstract void SendRejMessage(int p_rejected_message_id, string p_rejecting_comment);

        public override abstract bool Equals(object obj);

        protected void RemoveThisDialog()
        {
            remove_dialog_from_manager(dialog_id);
        }
    }

    public abstract class Se_AuthDialog : BaseDialog
    {
        protected string user_name;
        protected IsLoggedIn is_logged_in;
        protected AuthSend send_to_user;


        public string Get_user_name
        {
            get { return user_name; }
        }

        protected void BaseAuthConstruct(int p_dialog_id, TypeOfDialog p_dialog_type, Remove p_remove_dialog_from_manager, int p_max_retry_counts
            , string p_user_name, IsLoggedIn p_is_logged_in, AuthSend p_send_to_user)
        {
            base.BaseConstruct(p_dialog_id, p_dialog_type, p_remove_dialog_from_manager, p_max_retry_counts);
            user_name = p_user_name;
            is_logged_in = p_is_logged_in;
            send_to_user = p_send_to_user;
        }        

        protected override void SendAccMessage(int p_accepted_message_id)
        {
            Di_Mess_Rec_AcceptMessage temp_accept_message = new Di_Mess_Rec_AcceptMessage(p_accepted_message_id);
            Di_Mess_ReceiptMessage receipt_object = new Di_Mess_ReceiptMessage(ReceiptStatus.Accepted, temp_accept_message);
            DialogMessageForClient accept_message = new DialogMessageForClient(HelperFunctions.GetGUID(), Get_dialog_id, Get_last_message_number + 1
                , Get_dialog_type, receipt_object, TypeOfDialogMessage.ReceiptMessage);
            last_message_number = accept_message.Get_message_number_in_dialog;
            send_to_user(this.user_name, accept_message);
        }
        protected override void SendRejMessage(int p_rejected_message_id, string p_rejecting_comment)
        {
            Di_Mess_Rec_RejectMessage temp_reject_message = new Di_Mess_Rec_RejectMessage(p_rejected_message_id, p_rejecting_comment);
            Di_Mess_ReceiptMessage receipt_object = new Di_Mess_ReceiptMessage(ReceiptStatus.Rejected, temp_reject_message);
            DialogMessageForClient reject_message = new DialogMessageForClient(HelperFunctions.GetGUID(), Get_dialog_id, Get_last_message_number + 1
                , Get_dialog_type, receipt_object, TypeOfDialogMessage.ReceiptMessage);
            last_message_number = reject_message.Get_message_number_in_dialog;
            send_to_user(user_name, reject_message);
        }
        protected override void SendFormalDialogMessage(DialogMessageForClient p_message_to_be_sent)
        {
            send_to_user(user_name, p_message_to_be_sent);
        }

    }
    public abstract class Se_UnAuthDialog : BaseDialog
    {
        protected int thread_id;
        public int Get_thread_id
        {
            get { return thread_id; }
        }

        protected IsThereUnauthWorkerThread is_there_unauththread;
        protected UnAuthSend send_to_user;

        protected void UnAuthBaseConstruct(int p_dialog_id, TypeOfDialog p_dialog_type, Remove p_remove_dialog_from_manager, int p_max_retry_counts
            , int p_thread_id, IsThereUnauthWorkerThread p_is_there_unauththread, UnAuthSend p_send_to_user)
        {
            base.BaseConstruct(p_dialog_id, p_dialog_type, p_remove_dialog_from_manager, p_max_retry_counts);
            thread_id = p_thread_id;
            is_there_unauththread = p_is_there_unauththread;
            send_to_user = p_send_to_user;
        }

        protected override void SendAccMessage(int p_accepted_message_id)
        {
            Di_Mess_Rec_AcceptMessage temp_accept_message = new Di_Mess_Rec_AcceptMessage(p_accepted_message_id);
            Di_Mess_ReceiptMessage receipt_object = new Di_Mess_ReceiptMessage(ReceiptStatus.Accepted, temp_accept_message);
            DialogMessageForClient accept_message = new DialogMessageForClient(HelperFunctions.GetGUID(), Get_dialog_id, Get_last_message_number + 1
                , Get_dialog_type, receipt_object, TypeOfDialogMessage.ReceiptMessage);
            last_message_number = accept_message.Get_message_number_in_dialog;
            send_to_user(this.thread_id, accept_message);
        }
        protected override void SendRejMessage(int p_rejected_message_id, string p_rejecting_comment)
        {
            Di_Mess_Rec_RejectMessage temp_reject_message = new Di_Mess_Rec_RejectMessage(p_rejected_message_id, p_rejecting_comment);
            Di_Mess_ReceiptMessage receipt_object = new Di_Mess_ReceiptMessage(ReceiptStatus.Rejected, temp_reject_message);
            DialogMessageForClient reject_message = new DialogMessageForClient(HelperFunctions.GetGUID(), Get_dialog_id, Get_last_message_number + 1
                , Get_dialog_type, receipt_object, TypeOfDialogMessage.ReceiptMessage);
            last_message_number = reject_message.Get_message_number_in_dialog;
            send_to_user(thread_id, reject_message);
        }
        protected override void SendFormalDialogMessage(DialogMessageForClient p_message_to_be_sent)
        {
            send_to_user(thread_id, p_message_to_be_sent);
        }
    }


    public class Di_Server_LoginRequest : Se_UnAuthDialog
    {
        Login server_login;

        public Di_Server_LoginRequest(int p_dialog_id, int p_thread_id, Remove p_remove_dialog_from_manager, Login p_server_login, UnAuthSend p_send_to_user
            , IsThereUnauthWorkerThread p_is_there_unauththread)
        {
            base.UnAuthBaseConstruct(p_dialog_id, TypeOfDialog.LoginRequest, p_remove_dialog_from_manager, 6, p_thread_id, p_is_there_unauththread, p_send_to_user);
            server_login = p_server_login;
            level_counts = 1;
            all_dialog_levels.Add(1, new DialogLevelsInformation(new ALevelOfDialog(LevelOneFunction), DialogLevelType.WaitingForMessageReceive));
            Start();
        }

        private void LevelOneFunction()
        {
            if (last_message_received.Get_message_object_type != TypeOfDialogMessage.LoginRequestData)
            {
                LevelReject("message object id not correct.");
                return;
            }

            Di_Mess_LoginRequestData login_data = (Di_Mess_LoginRequestData)Get_last_message_received.Get_message_object;
            Se_BaseBooleanFunctionResult login_result = null;
            try
            {
                login_result = server_login(thread_id, login_data.Get_user_name, login_data.Get_password);
            }
            catch (SqlException)
            {
                LevelReject("A problem ocurred. try again");
                return;
            }
            catch (System.Data.DataException)
            {
                LevelReject("A problem ocurred. try again");
                return;
            }

            if (login_result is Se_BooleanFunctionAccResult)
            {
                LevelAccept();
                return;
            }
            else if (login_result is Se_BooleanFunctionRejResult)
            {
                LevelReject(((Se_BooleanFunctionRejResult)login_result).get_reject_comment);
                return;
            }

            return;


        }

        public override bool Equals(object obj)
        {
            Di_Server_LoginRequest temp_dialog_object = null;
            try
            {
                temp_dialog_object = (Di_Server_LoginRequest)obj;
            }
            catch (Exception)
            {
                return false;
            }
            if (thread_id == temp_dialog_object.thread_id || dialog_id == temp_dialog_object.dialog_id)
            {
                return true;
            }
            return false;

        }

    }

    public class Di_Server_LoginDataRequest : Se_AuthDialog
    {
        GetFriendsList get_user_friends_list;
        GetUserStatus get_user_status;
        GetPublicChatIds get_public_chat_ids;
        GetOfflineMessages get_all_user_offline_message;
        GetAllAgreementInvitation get_all_user_agreement_invitation;

        public Di_Server_LoginDataRequest(int p_dialog_id, string p_user_name, AuthSend p_send_to_user, GetFriendsList p_get_user_friends_list, GetUserStatus p_get_user_status,
            GetPublicChatIds p_get_public_chat_ids, GetOfflineMessages p_get_all_user_offline_message, GetAllAgreementInvitation p_get_all_user_agreement_invitation,
            Remove p_remove_dialog_from_manager, IsLoggedIn p_is_logged_in)
        {

            base.BaseAuthConstruct(p_dialog_id, TypeOfDialog.LoginDataRequest, p_remove_dialog_from_manager, 3, p_user_name, p_is_logged_in, p_send_to_user);

            get_user_friends_list = p_get_user_friends_list;
            get_user_status = p_get_user_status;
            get_public_chat_ids = p_get_public_chat_ids;
            get_all_user_offline_message = p_get_all_user_offline_message;
            get_all_user_agreement_invitation = p_get_all_user_agreement_invitation;

            level_counts = 6;
            all_dialog_levels.Add(1, new DialogLevelsInformation(new ALevelOfDialog(FirstLevelFunction), DialogLevelType.WaitingForMessageReceive));
            all_dialog_levels.Add(2, new DialogLevelsInformation(new ALevelOfDialog(SecondLevelFunction), DialogLevelType.SendingAMessage));
            all_dialog_levels.Add(3, new DialogLevelsInformation(new ALevelOfDialog(ThirdLevelFunction), DialogLevelType.SendingAMessage));
            all_dialog_levels.Add(4, new DialogLevelsInformation(new ALevelOfDialog(FourthLevelFunction), DialogLevelType.SendingAMessage));
            all_dialog_levels.Add(5, new DialogLevelsInformation(new ALevelOfDialog(FifthLevelFunction), DialogLevelType.SendingAMessage));
            all_dialog_levels.Add(6, new DialogLevelsInformation(new ALevelOfDialog(SixthLevelFunction), DialogLevelType.SendingAMessage));

            Start();
        }


        private void FirstLevelFunction()
        {
            if (last_message_received.Get_message_object_type != TypeOfDialogMessage.LoginDataRequestMessage)
            {
                LevelReject("message type is not correct.");
                return;
            }
            LevelAccept();
            return;
        }

        private void SecondLevelFunction()
        {
            Di_Mess_AccountInformation account_information_message_object = new Di_Mess_AccountInformation(user_name);
            Send(TypeOfDialogMessage.AccountInformation, account_information_message_object);
            return;
        }

        private void ThirdLevelFunction()
        {
            int j = 0;
            while (j < 3)
            {
                try
                {
                    List<string> all_friends_list = new List<string>();
                    all_friends_list = get_user_friends_list(user_name);
                    List<PersonStatus> all_friends_status = new List<PersonStatus>();
                    foreach (string temp_friend_name in all_friends_list)
                    {
                        all_friends_status.Add(new PersonStatus(temp_friend_name, get_user_status(temp_friend_name)));
                    }
                    Di_Mess_FriendsListAndStatus friends_list_and_status_message_object = new Di_Mess_FriendsListAndStatus(all_friends_status);
                    Send(TypeOfDialogMessage.FriendsListAndStatus, friends_list_and_status_message_object);
                    j = 0;
                    break;
                }
                catch (SqlException)
                {
                    j++;
                    continue;
                }
            }
            if (j > 2)
            {
                CancelDialog("a problem occured. try again");
                return;
            }
            return;
        }

        private void FourthLevelFunction()
        {
            List<int> public_chat_ids = get_public_chat_ids();
            Di_Mess_PublicChatIds public_chat_info_message_object = new Di_Mess_PublicChatIds(public_chat_ids);
            Send(TypeOfDialogMessage.PublicChatIds, public_chat_info_message_object);
            return;
        }

        private void FifthLevelFunction()
        {
            List<AgreementInvitationInfo> all_user_agreement_invitation = new List<AgreementInvitationInfo>();

            int j = 0;
            while (j < 3)
            {
                try
                {
                    all_user_agreement_invitation = get_all_user_agreement_invitation(user_name);
                    j = 0;
                    break;
                }
                catch (SqlException)
                {
                    j++;
                    continue;
                }
            }
            if (j > 2)
            {
                CancelDialog("a problem occured. try again");
                return;
            }
            Di_Mess_InviteToAgreemenstInfo invite_to_agreement_message_object = new Di_Mess_InviteToAgreemenstInfo(all_user_agreement_invitation);
            Send(TypeOfDialogMessage.InviteToAgreementInfo, invite_to_agreement_message_object);
            return;
        }

        private void SixthLevelFunction()
        {
            List<OfflineMessage> all_users_offline_messages = new List<OfflineMessage>();
            int j = 0;
            while (j < 3)
            {
                try
                {
                    all_users_offline_messages = get_all_user_offline_message(user_name);
                    j = 0;
                    break;
                }
                catch (SqlException)
                {
                    j++;
                    continue;
                }
            }
            if (j > 2)
            {
                CancelDialog("a problem occured. try again");
                return;
            }

            Di_Mess_OfflineMessages user_offline_messages_message_object = new Di_Mess_OfflineMessages(all_users_offline_messages);
            Send(TypeOfDialogMessage.OfflineMessages, user_offline_messages_message_object);
            return;
        }


        public override bool Equals(object obj)
        {
            Di_Server_LoginDataRequest temp_dialog_object = null;
            try
            {
                temp_dialog_object = (Di_Server_LoginDataRequest)obj;
            }
            catch (Exception)
            {
                return false;
            }
            if (dialog_id == temp_dialog_object.dialog_id || user_name == temp_dialog_object.user_name)
            {
                return true;
            }
            return false;
        }

    }

    public class Di_Server_CreateAddAgreement : Se_AuthDialog
    {
        CreateAddAgreement create_add_agreement;
        CreateOfflineMessage create_offline_message;
        string invited_user_name;

        public Di_Server_CreateAddAgreement(int p_dialog_id, string p_user_name, AuthSend p_send_to_user, Remove p_remove_dialog_from_manager, CreateAddAgreement p_create_add_agreement
            , CreateOfflineMessage p_create_offline_message, IsLoggedIn p_is_logged_in)
        {
            base.BaseAuthConstruct(p_dialog_id, TypeOfDialog.CreateAddAgreement, p_remove_dialog_from_manager, 3, p_user_name, p_is_logged_in, p_send_to_user);

            create_offline_message = p_create_offline_message;
            create_add_agreement = p_create_add_agreement;

            invited_user_name = "";

            level_counts = 1;
            all_dialog_levels.Add(1, new DialogLevelsInformation(new ALevelOfDialog(FirstLevelFunction), DialogLevelType.WaitingForMessageReceive));

            Start();
        }

        private void FirstLevelFunction()
        {
            if (last_message_received.Get_message_object_type != TypeOfDialogMessage.CreateAddAgreementRequest)
            {
                LevelReject("massage object type is not correct");
                return;
            }

            invited_user_name = ((Di_Mess_CreateAddAgreementRequest)last_message_received.Get_message_object).Get_user_id_to_add;
            Se_BaseBooleanFunctionResult add_request_result = null;

            int h = 0;
            while (h < 3)
            {
                try
                {
                    add_request_result = create_add_agreement(user_name, invited_user_name);
                    h = 0;
                    break;
                }
                catch (Exception)
                {
                    h++;
                    continue;
                }
            }
            if (h > 2)
            {
                LevelReject("a problem occured. please try again");
                return;
            }

            if (add_request_result is Se_BooleanFunctionAccResult)
            {
                LevelAccept();
                return;
            }
            else if (add_request_result is Se_BooleanFunctionRejResult)
            {
                LevelReject(((Se_BooleanFunctionRejResult)add_request_result).get_reject_comment);
                return;
            }
        }

        public override bool Equals(object obj)
        {
            Di_Server_CreateAddAgreement temp_dialog_object = null;
            try
            {
                temp_dialog_object = (Di_Server_CreateAddAgreement)obj;
            }
            catch (Exception)
            {
                return false;
            }
            if (dialog_id == temp_dialog_object.dialog_id || (user_name == temp_dialog_object.user_name && invited_user_name == temp_dialog_object.invited_user_name))
            {
                return true;
            }
            return false;
        }
    }

    public class Di_Server_GetAgreementAnswer : Se_AuthDialog 
    {
        GetAgreementAnswer get_agreement_answer;
        int agreement_id;
        bool agreement_answer;
        public Di_Server_GetAgreementAnswer(int p_dialog_id, string p_user_name, AuthSend p_send_to_user, Remove p_remove_dialog_from_manager, GetAgreementAnswer p_get_agreement_answer,
            IsLoggedIn p_is_logged_in)
        {
            base.BaseAuthConstruct(p_dialog_id, TypeOfDialog.GetAgreementAnswer, p_remove_dialog_from_manager, 3, p_user_name, p_is_logged_in, p_send_to_user);
            get_agreement_answer = p_get_agreement_answer;
            agreement_id = 0;
            agreement_answer = false;
            level_counts = 1;
            all_dialog_levels.Add(1, new DialogLevelsInformation(new ALevelOfDialog(LevelOneFunction), DialogLevelType.WaitingForMessageReceive));
            Start();
        }

        private void LevelOneFunction()
        {
            if (last_message_received.Get_message_object_type != TypeOfDialogMessage.AgreementAnswer)
            {
                LevelReject("message object is not correct.");
                return;
            }
            if (((Di_Mess_AgreementAnswer)Get_last_message_received.Get_message_object).Get_agreement_type != TypeOfAgreement.Add)
            {
                LevelReject("message object is not correct.");
                return;
            }
            agreement_id = ((Di_Mess_AgreementAnswer)last_message_received.Get_message_object).Get_agreement_id;
            agreement_answer = ((Di_Mess_AgreementAnswer)last_message_received.Get_message_object).Get_answer;
            try
            {
                get_agreement_answer(user_name, agreement_id, agreement_answer, TypeOfAgreement.Add);
            }
            catch (Exception)
            {
                LevelReject("a problem occured. try again");
                return;
            }
            LevelAccept();
        }

        public override bool Equals(object obj)
        {
            Di_Server_GetAgreementAnswer temp_dialog_object = null;
            try
            {
                temp_dialog_object = (Di_Server_GetAgreementAnswer)obj;
            }
            catch (Exception)
            {
                return false;
            }
            if (dialog_id == temp_dialog_object.dialog_id || (user_name == temp_dialog_object.user_name && agreement_id == temp_dialog_object.agreement_id))
            {
                return true;
            }
            return false;
        }


    }

    public class Di_Server_ServerReadOfflineMessagesInform : Se_AuthDialog
    {
        OfflineMessagesReadInform offline_message_read_inform;
        List<int> messages_ids;

        public Di_Server_ServerReadOfflineMessagesInform(int p_dialog_id, Remove p_remove_dialog_from_manager
            , string p_user_name, IsLoggedIn p_is_logged_in, AuthSend p_send_to_user, OfflineMessagesReadInform p_offline_message_read_inform)
        {
            base.BaseAuthConstruct(p_dialog_id, TypeOfDialog.ServerReadOfflineMessagesInform, p_remove_dialog_from_manager, 3, p_user_name, p_is_logged_in, p_send_to_user);

            offline_message_read_inform = p_offline_message_read_inform;
            messages_ids = new List<int>();

            level_counts = 1;
            all_dialog_levels.Add(1, new DialogLevelsInformation(new ALevelOfDialog(FirstLevelfunction), DialogLevelType.WaitingForMessageReceive));

            Start();
        }

        private void FirstLevelfunction()
        {
            if (last_message_received.Get_message_object_type != TypeOfDialogMessage.ServerReadOfflineMessagesInform)
            {
                LevelReject("Message Object is not correct.");
                return;
            }

            messages_ids = ((Di_Mess_ServerReadOfflineMessagesInform)last_message_received.Get_message_object).Get_message_ids;

            int h = 0;
            while (h < 3)
            {
                try
                {
                    offline_message_read_inform(user_name, messages_ids);
                    LevelAccept();
                    h = 0;
                    break;
                }
                catch (SqlException)
                {
                    h++;
                    continue;
                }
            }
            if (h > 2)
            {
                LevelReject("a problem occured. try again.");
                return;
            }
        }

        public override bool Equals(object obj)
        {
            Di_Server_ServerReadOfflineMessagesInform temp_dialog_object = null;
            try
            {
                temp_dialog_object = (Di_Server_ServerReadOfflineMessagesInform)obj;
            }
            catch (Exception)
            {
                return false;
            }
            bool t_bool = false;
            if (temp_dialog_object.messages_ids.Count == messages_ids.Count)
            {
                t_bool = true;
                foreach (int t_id in temp_dialog_object.messages_ids)
                {
                    if (!messages_ids.Contains(t_id))
                    {
                        t_bool = false;
                        break;
                    }
                }
            }
            if (dialog_id == temp_dialog_object.dialog_id || (t_bool && user_name == temp_dialog_object.user_name))
            {
                return true;
            }
            return false;
        }


    }

    public class Di_Server_ClientLeaveChatRequest : Se_AuthDialog
    {
        ServerInformChatLeave server_inform_chat_leave;

        int chat_id;
        TypeOfChat chat_type;

        public Di_Server_ClientLeaveChatRequest(int p_dialog_id, string p_user_name, AuthSend p_send_to_user, Remove p_remove_dialog_from_manager, ServerInformChatLeave p_server_inform_chat_leave
            , IsLoggedIn p_is_logged_in)
        {
            base.BaseAuthConstruct(p_dialog_id, TypeOfDialog.ClientLeaveChatRequest, p_remove_dialog_from_manager, 3, p_user_name, p_is_logged_in, p_send_to_user);

            server_inform_chat_leave = p_server_inform_chat_leave;
            chat_id = 0;
            chat_type = TypeOfChat.Public;

            level_counts = 1;
            all_dialog_levels.Add(1, new DialogLevelsInformation(new ALevelOfDialog(LevelOneFunction), DialogLevelType.WaitingForMessageReceive));
            Start();
        }

        private void LevelOneFunction()
        {
            if (last_message_received.Get_message_object_type != TypeOfDialogMessage.ClientLeaveChatRequest)
            {
                LevelReject("messageobject is  not correct");
                return;
            }

            Di_Mess_ClientLeaveChatRequest temp_leave_chat_request = ((Di_Mess_ClientLeaveChatRequest)last_message_received.Get_message_object);

            chat_id = temp_leave_chat_request.Get_chat_id;
            chat_type = temp_leave_chat_request.Get_chat_type;

            server_inform_chat_leave(user_name, temp_leave_chat_request.Get_chat_id, temp_leave_chat_request.Get_chat_type);
            LevelAccept();
        }

        public override bool Equals(object obj)
        {
            Di_Server_ClientLeaveChatRequest temp_dialog_object = null;
            try
            {
                temp_dialog_object = (Di_Server_ClientLeaveChatRequest)obj;
            }
            catch (Exception)
            {
                return false;
            }
            if (dialog_id == temp_dialog_object.Get_dialog_id || (chat_id == temp_dialog_object.chat_id
                && user_name == temp_dialog_object.user_name && chat_type == temp_dialog_object.chat_type))
            {
                return true;
            }
            return false;
        }

    }

    public class Di_Server_ClientSomeoneLeftChatInform : Se_AuthDialog 
    {
        string user_left_chat_name;
        int chat_id;
        TypeOfChat chat_type;

        public Di_Server_ClientSomeoneLeftChatInform(int p_dialog_id, string p_user_name, AuthSend p_send_to_user, Remove p_remove_dialog_from_manager
            , string p_user_left_chat_name, int p_chat_id, IsLoggedIn p_is_logged_in, TypeOfChat p_chat_type)
        {
            base.BaseAuthConstruct(p_dialog_id, TypeOfDialog.ClientSomeoneLeftChatInform, p_remove_dialog_from_manager, 3, p_user_name, p_is_logged_in, p_send_to_user);

            user_left_chat_name = p_user_left_chat_name;
            chat_type = p_chat_type;
            chat_id = p_chat_id;

            level_counts = 1;
            all_dialog_levels.Add(1, new DialogLevelsInformation(new ALevelOfDialog(LevelOneFunction), DialogLevelType.SendingAMessage));
        }

        public void StartDialog()
        {
            Start();
        }

        private void LevelOneFunction()
        {
            Di_Mess_SomeoneLeftTheChat left_chat_messge_object = new Di_Mess_SomeoneLeftTheChat(user_left_chat_name, chat_id, chat_type);
            Send(TypeOfDialogMessage.SomeoneLeftTheChat, left_chat_messge_object);
        }

        public override bool Equals(object obj)
        {
            Di_Server_ClientSomeoneLeftChatInform temp_dialog_object = null;
            try
            {
                temp_dialog_object = (Di_Server_ClientSomeoneLeftChatInform)obj;
            }
            catch (Exception)
            {
                return false;
            }
            if (dialog_id == temp_dialog_object.Get_dialog_id || (user_name == temp_dialog_object.user_name &&
                user_left_chat_name == temp_dialog_object.user_left_chat_name && chat_id == temp_dialog_object.chat_id))
            {
                return true;
            }
            return false;
        }
    }

    public class Di_server_InformEjectedChatUser : Se_AuthDialog
    {
        int chat_id_user_ejected_from;
        string ejecting_comment;
        TypeOfChat chat_type;

        public Di_server_InformEjectedChatUser(int p_dialog_id, string p_user_name, AuthSend p_send_to_user, Remove p_remove_dialog_from_manager,
            IsLoggedIn p_is_logged_in, int p_chat_id_user_ejected_from, string p_ejecting_comment, TypeOfChat p_chat_type)
        {
            base.BaseAuthConstruct(p_dialog_id, TypeOfDialog.InformEjectedChatUser, p_remove_dialog_from_manager, 3, p_user_name, p_is_logged_in, p_send_to_user);

            chat_id_user_ejected_from = p_chat_id_user_ejected_from;
            ejecting_comment = p_ejecting_comment;
            chat_type = p_chat_type;

            level_counts = 1;
            all_dialog_levels.Add(1, new DialogLevelsInformation(new ALevelOfDialog(LevelOneFunction), DialogLevelType.SendingAMessage));
        }

        public void StartDialog()
        {
            Start();
        }

        private void LevelOneFunction()
        {
            Di_Mess_InformEjectedChatUser ejecting_message_object = new Di_Mess_InformEjectedChatUser(chat_id_user_ejected_from, ejecting_comment, chat_type);
            Send(TypeOfDialogMessage.InformEjectedChatUser, ejecting_message_object);
            return;
        }

        public override bool Equals(object obj)
        {
            Di_server_InformEjectedChatUser temp_dialog_object = null;
            try
            {
                temp_dialog_object = (Di_server_InformEjectedChatUser)obj;
            }
            catch (Exception)
            {
                return false;
            }

            if (dialog_id == temp_dialog_object.Get_dialog_id || (user_name == temp_dialog_object.user_name
                && chat_id_user_ejected_from == temp_dialog_object.chat_id_user_ejected_from  && chat_type == temp_dialog_object.chat_type))
            {
                return true;
            }

            return false;
        }
    }

    public class Di_Server_CreatePrivateChatRequest : Se_AuthDialog
    {
        OnlineAreFriends are_friends;
        CreatePrivateChat create_private_chat;
        string invited_person_user_name;
        IsTherePrivateChat is_there_pr_chat;
        StartClientCreatedPrivateChatInform start_chat_with_other_person;
        int chat_id;

        public Di_Server_CreatePrivateChatRequest(int p_dialog_id, string p_user_name, AuthSend p_send_to_user, Remove p_remove_dialog_from_manager
            , IsLoggedIn p_is_logged_in, OnlineAreFriends p_are_friends, CreatePrivateChat p_create_private_chat
            , StartClientCreatedPrivateChatInform p_start_chat_with_other_person, IsTherePrivateChat p_is_there_pr_chat)
        {
            base.BaseAuthConstruct(p_dialog_id, TypeOfDialog.CreatePrivateChatRequest, p_remove_dialog_from_manager, 1, p_user_name, p_is_logged_in, p_send_to_user);

            chat_id = 0;
            invited_person_user_name = "";
            create_private_chat = p_create_private_chat;
            start_chat_with_other_person = p_start_chat_with_other_person;
            is_there_pr_chat = p_is_there_pr_chat;
            are_friends = p_are_friends;

            level_counts = 2;
            all_dialog_levels.Add(1, new DialogLevelsInformation(new ALevelOfDialog(FirstLevelFunction), DialogLevelType.WaitingForMessageReceive));
            all_dialog_levels.Add(2, new DialogLevelsInformation(new ALevelOfDialog(SecondLevelFunction), DialogLevelType.SendingAMessage));
            Start();
        }

        private void FirstLevelFunction()
        {
            if (last_message_received.Get_message_object_type != TypeOfDialogMessage.StartPrivateChatRequest)
            {
                LevelReject("message object is not correct.");
                return;
            }

            Di_Mess_StartPrivateChatRequest request_message_object = ((Di_Mess_StartPrivateChatRequest)last_message_received.Get_message_object);
            invited_person_user_name = request_message_object.Get_invited_person_user_name;

            if (user_name == invited_person_user_name)
            {
                CancelDialog("this is not possible to create private chat with just one user ");
                return;
            }
            if (!is_logged_in(invited_person_user_name))
            {
                CancelDialog("this person is not logged in");
                return;
            }

            Se_BaseBooleanFunctionResult are_friends_result = null;
            are_friends_result = are_friends(user_name, invited_person_user_name);

            if (are_friends_result is Se_BooleanFunctionRejResult)
            {
                CancelDialog(((Se_BooleanFunctionRejResult)are_friends_result).get_reject_comment);
                return;
            }

            Se_BaseIntFunctionResult chat_create_request_result = create_private_chat(user_name, invited_person_user_name);
            if (chat_create_request_result is Se_IntFunctionAccResult)
            {
                Di_Mess_PrivateChatInfo created_chat_info = new Di_Mess_PrivateChatInfo(((Se_IntFunctionAccResult)chat_create_request_result).get_message_content);
                chat_id = created_chat_info.Get_chat_id;
                start_chat_with_other_person(invited_person_user_name, user_name, created_chat_info.Get_chat_id);
                LevelAccept();
                return;
            }
            else if (chat_create_request_result is Se_IntFunctionRejResult)
            {
                LevelReject(((Se_IntFunctionRejResult)chat_create_request_result).Get_reject_comment);
                return;
            }
        }

        private void SecondLevelFunction()
        {
            if (!is_logged_in(invited_person_user_name))
            {
                CancelDialog("this person is not looge in");
                return;
            }

            if (!is_logged_in(user_name))
            {
                End();
                return;
            }

            if (!is_there_pr_chat(chat_id))
            {
                CancelDialog("this chat deleted.");
                return;
            }

            Di_Mess_PrivateChatInfo chat_info = new Di_Mess_PrivateChatInfo(chat_id);
            Send(TypeOfDialogMessage.PrivateChatInfo, chat_info);
        }

        public override bool Equals(object obj)
        {
            Di_Server_CreatePrivateChatRequest temp_dialog_object = null;
            try
            {
                temp_dialog_object = (Di_Server_CreatePrivateChatRequest)obj;
            }
            catch (Exception)
            {
                return false;
            }
            if (dialog_id == temp_dialog_object.Get_dialog_id || (user_name == temp_dialog_object.user_name
                && invited_person_user_name == temp_dialog_object.invited_person_user_name))
            {
                return true;
            }
            return false;
        }

    }

    public class Di_Server_ClientJoinPublicChatRequest : Se_AuthDialog
    {
        JoinPublicChatRequest join_public_chat_request;
        IsUserInPublicChat is_user_in_pu_chat;
        GetPublicChatUsersList get_users_list;
        int chat_id;

        public Di_Server_ClientJoinPublicChatRequest(int p_dialog_id, string p_user_name, AuthSend p_send_to_user, Remove p_remove_dialog_from_manager
            , JoinPublicChatRequest p_join_public_chat_request, IsLoggedIn p_is_logged_in, IsUserInPublicChat p_is_user_in_pu_chat, GetPublicChatUsersList p_get_users_list)
        {
            base.BaseAuthConstruct(p_dialog_id, TypeOfDialog.ClientJoinPublicChatRequest, p_remove_dialog_from_manager, 1, p_user_name, p_is_logged_in, p_send_to_user);

            chat_id = 0;
            get_users_list = p_get_users_list;
            is_user_in_pu_chat = p_is_user_in_pu_chat;
            join_public_chat_request = p_join_public_chat_request;

            level_counts = 2;
            all_dialog_levels.Add(1, new DialogLevelsInformation(new ALevelOfDialog(FirstLevelFunction), DialogLevelType.WaitingForMessageReceive));
            all_dialog_levels.Add(2, new DialogLevelsInformation(new ALevelOfDialog(SecondLevelFunction), DialogLevelType.SendingAMessage));

            Start();
        }

        private void FirstLevelFunction()
        {
            if (last_message_received.Get_message_object_type != TypeOfDialogMessage.JoinPublicChatRequest)
            {
                CancelDialog("message object type is not valid.");
                return;
            }

            Di_Mess_JoinPublicChatRequest request_message_object = ((Di_Mess_JoinPublicChatRequest)last_message_received.Get_message_object);
            chat_id = request_message_object.Get_public_chat_id;
            Se_BaseBooleanFunctionResult join_public_chat_req_answer = join_public_chat_request(user_name, chat_id);

            if (join_public_chat_req_answer is Se_BooleanFunctionAccResult)
            {
                LevelAccept();
                return;
            }
            else if (join_public_chat_req_answer is Se_BooleanFunctionRejResult)
            {
                LevelReject(((Se_BooleanFunctionRejResult)join_public_chat_req_answer).get_reject_comment);
                return;
            }
        }

        private void SecondLevelFunction()
        {
            if (!is_logged_in(user_name))
            {
                End();
                return;
            }
            if (!is_user_in_pu_chat(user_name, chat_id))
            {
                CancelDialog("you ejected from this public chat");
                return;
            }
            List<string> all_users_list = get_users_list(chat_id);
            Di_Mess_PublicChatUsersIds public_chat_info_message_object = new Di_Mess_PublicChatUsersIds(all_users_list);
            Send(TypeOfDialogMessage.PublicChatUsersIds, public_chat_info_message_object);
            return;
        }

        public override bool Equals(object obj)
        {
            Di_Server_ClientJoinPublicChatRequest temp_dialog_object = null;
            try
            {
                temp_dialog_object = (Di_Server_ClientJoinPublicChatRequest)obj;
            }
            catch (Exception)
            {
                return false;
            }
            if (dialog_id == temp_dialog_object.Get_dialog_id || (user_name == temp_dialog_object.user_name && chat_id == temp_dialog_object.chat_id))
            {
                return true;
            }
            return false;
        }
    }

    public class Di_Server_ClientSomeoneJoinedChatInform : Se_AuthDialog
    {
        string new_person_user_name;
        int chat_id;
        TypeOfChat chat_type;

        public Di_Server_ClientSomeoneJoinedChatInform(int p_dialog_id, string p_user_name, AuthSend p_send_to_user, Remove p_remove_dialog_from_manager,
            IsLoggedIn p_is_logged_in, string p_new_person_user_name, int p_chat_id, TypeOfChat p_chat_type)
        {
            base.BaseAuthConstruct(p_dialog_id, TypeOfDialog.ClientSomeoneJoinedChatInform, p_remove_dialog_from_manager, 1, p_user_name, p_is_logged_in, p_send_to_user);

            new_person_user_name = p_new_person_user_name;
            chat_type = p_chat_type;
            chat_id = p_chat_id;

            level_counts = 1;
            all_dialog_levels.Add(1, new DialogLevelsInformation(new ALevelOfDialog(LevelOneFunction), DialogLevelType.SendingAMessage));
        }

        public void StartDialog()
        {
            Start();
        }

        private void LevelOneFunction()
        {
            Di_Mess_SomeoneJoinedTheChat someone_joined_chat_message_object = new Di_Mess_SomeoneJoinedTheChat(new_person_user_name, chat_id, chat_type);
            Send(TypeOfDialogMessage.SomeoneJoinedTheChat, someone_joined_chat_message_object);
        }

        public override bool Equals(object obj)
        {
            Di_Server_ClientSomeoneJoinedChatInform temp_dialog_object = null;
            try
            {
                temp_dialog_object = (Di_Server_ClientSomeoneJoinedChatInform)obj;
            }
            catch (Exception)
            {
                return false;
            }
            if (dialog_id == temp_dialog_object.Get_dialog_id || (user_name == temp_dialog_object.user_name && chat_id == temp_dialog_object.chat_id
                && new_person_user_name == temp_dialog_object.new_person_user_name && chat_type == temp_dialog_object.chat_type))
            {
                return true;
            }
            return false;
        }

    }

    public class Di_Server_ClientFriendChangedStatusInform : Se_AuthDialog
    {
        PersonStatus friend_changed_status;
        public Di_Server_ClientFriendChangedStatusInform(int p_dialog_id, string p_user_name, Remove p_remove_dialog_from_manager, IsLoggedIn p_is_logged_in
            , AuthSend p_send_to_user, string p_user_changed_status_name, UserStatus p_user_new_status)
        {
            base.BaseAuthConstruct(p_dialog_id, TypeOfDialog.ClientFriendChangedStatusInform, p_remove_dialog_from_manager, 3, p_user_name, p_is_logged_in, p_send_to_user);

            friend_changed_status = new PersonStatus(p_user_changed_status_name, p_user_new_status);

            level_counts = 1;
            all_dialog_levels.Add(1, new DialogLevelsInformation(new ALevelOfDialog(FirstLevelFunction), DialogLevelType.SendingAMessage));
        }

        public void StartDialog()
        {
            Start();
        }

        private void FirstLevelFunction()
        {
            Di_Mess_FriendChangeStatus friend_changed_status_dialog_message_object = new Di_Mess_FriendChangeStatus(friend_changed_status);
            Send(TypeOfDialogMessage.FriendChangeStatus, friend_changed_status_dialog_message_object);
        }

        public override bool Equals(object obj)
        {
            Di_Server_ClientFriendChangedStatusInform temp_dialog_object = null;
            try
            {
                temp_dialog_object = (Di_Server_ClientFriendChangedStatusInform)obj;
            }
            catch (Exception)
            {
                return false;
            }
            if (dialog_id == temp_dialog_object.Get_dialog_id || (user_name == temp_dialog_object.user_name &&
                friend_changed_status.Get_user_name == temp_dialog_object.friend_changed_status.Get_user_name &&
                friend_changed_status.Get_user_status == temp_dialog_object.friend_changed_status.Get_user_status))
            {
                return true;
            }
            return false;
        }
    }

    public class Di_Server_SendToClinetFormalMessage : Se_AuthDialog 
    {
        string sender_user_name;
        string message_text;
        int message_id;

        public string Get_sender_user_name
        {
            get { return sender_user_name; }
        }
        public string Get_message_text
        {
            get { return message_text; }
        }

        public Di_Server_SendToClinetFormalMessage(int p_dialog_id, Remove p_remove_dialog_from_manager, string p_user_name,
            IsLoggedIn p_is_logged_in, AuthSend p_send_to_user, string psender_user_name, string p_messsage_text, int p_message_id)
        {
            base.BaseAuthConstruct(p_dialog_id, TypeOfDialog.SendToClinetFormalMessage, p_remove_dialog_from_manager, 3, p_user_name, p_is_logged_in, p_send_to_user);

            sender_user_name = psender_user_name;
            message_text = p_messsage_text;
            message_id = p_message_id;

            level_counts = 1;
            all_dialog_levels.Add(1, new DialogLevelsInformation(new ALevelOfDialog(FirstLevelFunction), DialogLevelType.SendingAMessage));
        }

        public void StartDialog()
        {
            Start();
        }

        private void FirstLevelFunction()
        {
            Di_Mess_ClientInformFormalMessage client_inform_formal_message = new Di_Mess_ClientInformFormalMessage(new FormalMessage(sender_user_name
                , message_text), message_id);
            Send(TypeOfDialogMessage.ClientInformFormalMessage, client_inform_formal_message);
            return;
        }

        public override bool Equals(object obj)
        {
            Di_Server_SendToClinetFormalMessage temp_dialog_object = null;
            try
            {
                temp_dialog_object = (Di_Server_SendToClinetFormalMessage)obj;
            }
            catch (Exception)
            {
                return false;
            }
            if (dialog_id == temp_dialog_object.Get_dialog_id || (user_name == temp_dialog_object.user_name && sender_user_name == temp_dialog_object.sender_user_name
                && message_text == temp_dialog_object.message_text))
            {
                return true;
            }
            return false;
        }

    }

    public class Di_Server_FormalMessageRequest : Se_AuthDialog
    {
        CreateFormalMessageRequest create_formal_message_request;
        IstherUser user_exist;
        CreateOfflineMessage create_offline_message;

        string receiver_user_name;
        string message_text;

        public Di_Server_FormalMessageRequest(int p_dialog_id, string p_user_name, AuthSend p_send_to_user, Remove p_remove_dialog_from_list,
            CreateFormalMessageRequest p_create_formal_message_request, IstherUser p_user_exist, IsLoggedIn p_is_logged_in, CreateOfflineMessage p_create_offline_message)
        {
            base.BaseAuthConstruct(p_dialog_id, TypeOfDialog.FormalMessageRequest, p_remove_dialog_from_list, 3, p_user_name, p_is_logged_in, p_send_to_user);

            create_formal_message_request = p_create_formal_message_request;
            user_exist = p_user_exist;
            create_offline_message = p_create_offline_message;
            receiver_user_name = "";
            message_text = "";

            level_counts = 1;
            dialog_type = TypeOfDialog.FormalMessageRequest;
            all_dialog_levels.Add(1, new DialogLevelsInformation(new ALevelOfDialog(FirstLevelFunction), DialogLevelType.WaitingForMessageReceive));

            Start();
        }

        private void FirstLevelFunction()
        {
            if (last_message_received.Get_message_object_type != TypeOfDialogMessage.ClientFormalMessageRequest)
            {
                LevelReject("message object is not valid");
                return;
            }

            receiver_user_name = ((Di_Mess_ClientFormalMessageRequest)Get_last_message_received.Get_message_object).Get_receiver_user_id;
            message_text = ((Di_Mess_ClientFormalMessageRequest)Get_last_message_received.Get_message_object).Get_message_text;

            bool bool_user_exist = false;

            if (user_name == receiver_user_name)
            {
                LevelReject("you cannot send message for yourself!");
                return;
            }

            int h = 0;
            while (h < 3)
            {
                try
                {
                    bool_user_exist = user_exist(receiver_user_name);
                    h = 0;
                    break;
                }
                catch (SqlException)
                {
                    h++;
                    continue;

                }
            }
            if (h > 2)
            {
                LevelReject("A problem occured. try again.");
                return;
            }

            if (!bool_user_exist)
            {
                LevelReject("This username not exist.");
                return;
            }

            Di_Mess_ClientFormalMessageRequest formal_message_request = ((Di_Mess_ClientFormalMessageRequest)last_message_received.Get_message_object);
            FormalMessage formal_message_for_send = new FormalMessage(user_name, formal_message_request.Get_message_text);

            h = 0;
            while (h < 3)
            {
                try
                {
                    create_formal_message_request(formal_message_for_send, receiver_user_name);
                    h = 0;
                    break;
                }
                catch (SqlException)
                {
                    h++;
                    continue;
                }
            }
            if (h > 2)
            {
                LevelReject("a problem occured. try again");
                return;
            }
            LevelAccept();

        }

        public override bool Equals(object obj)
        {
            Di_Server_FormalMessageRequest temp_dialog_object = null;
            try
            {
                temp_dialog_object = (Di_Server_FormalMessageRequest)obj;
            }
            catch (Exception)
            {
                return false;
            }
            if (dialog_id == temp_dialog_object.Get_dialog_id || (user_name == temp_dialog_object.user_name &&
            receiver_user_name == temp_dialog_object.receiver_user_name && message_text == temp_dialog_object.message_text))
            {
                return true;
            }
            return false;
        }
    } 

    public class Di_Server_ClientSignupRequest : Se_UnAuthDialog
    {
        SignUp signup_request;

        string requested_user_name;
        string requested_password;

        public Di_Server_ClientSignupRequest(int p_dialog_id, int p_thread_id, UnAuthSend p_send_to_user, Remove p_remove_dialog_from_manager, SignUp p_signup_request,
            IsThereUnauthWorkerThread p_is_there_unauththread)
        {
            base.UnAuthBaseConstruct(p_dialog_id, TypeOfDialog.ClientSignupRequest, p_remove_dialog_from_manager, 20, p_thread_id, p_is_there_unauththread, p_send_to_user);

            signup_request = p_signup_request;
            requested_password = "";
            requested_user_name = "";

            level_counts = 1;
            all_dialog_levels.Add(1, new DialogLevelsInformation(new ALevelOfDialog(LevelOneFunction), DialogLevelType.WaitingForMessageReceive));
            Start();
        }

        private void LevelOneFunction()
        {
            if (last_message_received.Get_message_object_type != TypeOfDialogMessage.SignUpRequestData)
            {
                LevelReject("message object is not valid");
                return;
            }

            Di_Mess_SignUpRequestData temp_signup_request_data = ((Di_Mess_SignUpRequestData)last_message_received.Get_message_object);
            requested_user_name = temp_signup_request_data.Get_user_id_to_Register;
            requested_password = temp_signup_request_data.Get_password_to_use;

            if (string.IsNullOrWhiteSpace(temp_signup_request_data.Get_password_to_use) || string.IsNullOrWhiteSpace(temp_signup_request_data.Get_user_id_to_Register))
            {
                LevelReject("Incorrect objects for username and/or password. at least one is null or empty or hase whitespace");
                return;
            }

            Se_BaseBooleanFunctionResult signup_request_result = null;
            try
            {
                signup_request_result = signup_request(temp_signup_request_data.Get_user_id_to_Register, temp_signup_request_data.Get_password_to_use);
            }
            catch (SqlException)
            {
                LevelReject("A problem occured. please try again");
                return;
            }
            if (signup_request_result is Se_BooleanFunctionAccResult)
            {
                LevelAccept();
                return;
            }
            else if (signup_request_result is Se_BooleanFunctionRejResult)
            {
                LevelReject(((Se_BooleanFunctionRejResult)signup_request_result).get_reject_comment);
                return;
            }
        }

        public override bool Equals(object obj)
        {
            Di_Server_ClientSignupRequest temp_dialog_object = null;
            try
            {
                temp_dialog_object = (Di_Server_ClientSignupRequest)obj;
            }
            catch (Exception)
            {
                return false;
            }
            if (dialog_id == temp_dialog_object.Get_dialog_id || (thread_id == temp_dialog_object.thread_id))
            {
                return true;
            }
            return false;
        }
    }

    public class Di_Server_ClientFriendListChangedInform : Se_AuthDialog
    {
        List<PersonStatus> new_friends_and_status;
        public Di_Server_ClientFriendListChangedInform(int p_dialog_id, Remove p_remove_dialog_from_manager, IsLoggedIn p_is_logged_in,
            AuthSend p_send_to_user, string p_user_name, List<PersonStatus> p_new_frineds_and_status)
        {
            base.BaseAuthConstruct(p_dialog_id, TypeOfDialog.ClientFriendListChangedInform, p_remove_dialog_from_manager, 3, p_user_name, p_is_logged_in, p_send_to_user);

            new_friends_and_status = p_new_frineds_and_status;

            level_counts = 1;
            all_dialog_levels.Add(1, new DialogLevelsInformation(new ALevelOfDialog(FirstlevelFunction), DialogLevelType.SendingAMessage));
        }

        public void StartDialog()
        {
            Start();
        }

        void FirstlevelFunction()
        {
            Di_Mess_FriendsListAndStatus friend_list_and_status_message_object = new Di_Mess_FriendsListAndStatus(new_friends_and_status);
            Send(TypeOfDialogMessage.FriendsListAndStatus, friend_list_and_status_message_object);
        }

        public override bool Equals(object obj)
        {
            Di_Server_ClientFriendListChangedInform temp_dialog_object = null;
            try
            {
                temp_dialog_object = (Di_Server_ClientFriendListChangedInform)obj;
            }
            catch (Exception)
            {
                return false;
            }

            bool t_bool = false;
            if (new_friends_and_status.Count == temp_dialog_object.new_friends_and_status.Count)
            {
                t_bool = true;
                foreach (PersonStatus t_person_status in temp_dialog_object.new_friends_and_status)
                {
                    if (!new_friends_and_status.Contains(t_person_status))
                    {
                        t_bool = false;
                        break;
                    }
                }
            }

            if (dialog_id == temp_dialog_object.Get_dialog_id || (user_name == temp_dialog_object.user_name && t_bool))
            {
                return true;
            }
            return false;
        }

    }

    public class Di_Server_ClientCreatedPrivateChatInform : Se_AuthDialog
    {
        string starter_user_name;
        int chat_id;

        GetPrivateChatInvitationAnswer get_private_chat_invitation_answer;
        IsTherePrivateChat is_there_private_chat;

        public Di_Server_ClientCreatedPrivateChatInform(int p_dialog_id, Remove p_remove_dialog_from_manager, IsLoggedIn p_is_logged_in,
            AuthSend p_send_to_user, string p_user_name, string p_starter_user_name, int p_chat_id, IsTherePrivateChat p_is_there_private_chat
            , GetPrivateChatInvitationAnswer p_get_private_chat_invitation_answer)
        {
            base.BaseAuthConstruct(p_dialog_id, TypeOfDialog.ClientCreatedPrivateChatInform, p_remove_dialog_from_manager, 3, p_user_name, p_is_logged_in, p_send_to_user);

            get_private_chat_invitation_answer = p_get_private_chat_invitation_answer;
            is_there_private_chat = p_is_there_private_chat;

            starter_user_name = p_starter_user_name;
            chat_id = p_chat_id;

            level_counts = 2;
            all_dialog_levels.Add(1, new DialogLevelsInformation(new ALevelOfDialog(FirstLevelFunction), DialogLevelType.SendingAMessage));
            all_dialog_levels.Add(2, new DialogLevelsInformation(new ALevelOfDialog(SecondLevelFunction), DialogLevelType.WaitingForMessageReceive));
        }

        public void StartDialog()
        {
            Start();
        }

        void FirstLevelFunction()
        {
            Di_Mess_CreatedPrivateChatInform create_private_chat_inform_message_object = new Di_Mess_CreatedPrivateChatInform(starter_user_name, chat_id);
            Send(TypeOfDialogMessage.CreatedPrivateChatInform, create_private_chat_inform_message_object);
        }

        private void SecondLevelFunction()
        {
            if (!is_there_private_chat(chat_id))
            {
                LevelAccept();
                return;
            }
            if (last_message_received.Get_message_object_type != TypeOfDialogMessage.PrivateChatInvitationAnswer)
            {
                LevelReject("message object is not correct.");
                return;
            }

            Di_Mess_PrivateChatInvitationAnswer private_chat_invitation_answer = ((Di_Mess_PrivateChatInvitationAnswer)last_message_received.Get_message_object);
            get_private_chat_invitation_answer(user_name, chat_id, private_chat_invitation_answer.Get_answer);
            LevelAccept();
            return;
        }

        public override bool Equals(object obj)
        {
            Di_Server_ClientCreatedPrivateChatInform temp_dialog_object = null;
            try
            {
                temp_dialog_object = (Di_Server_ClientCreatedPrivateChatInform)obj;
            }
            catch (Exception)
            {
                return false;
            }
            if (dialog_id == temp_dialog_object.Get_dialog_id || (user_name == temp_dialog_object.user_name && starter_user_name == temp_dialog_object.starter_user_name
                && chat_id == temp_dialog_object.chat_id))
            {
                return true;
            }
            return false;
        }
    }

    public class Di_Server_ClientInvitedAgreementInform : Se_AuthDialog
    {
        GetAUserAgreementInvitation get_a_user_agreement_invitation;

        string user_name;
        int agreement_id;

        public Di_Server_ClientInvitedAgreementInform(int p_dialog_id, Remove p_remove_dialog_from_manager, IsLoggedIn p_is_logged_in,
            AuthSend p_send_to_user, string p_user_name, int p_agreement_id, GetAUserAgreementInvitation p_get_a_user_agreement_invitation)
        {
            base.BaseAuthConstruct(p_dialog_id, TypeOfDialog.ClientInvitedAgreementInform, p_remove_dialog_from_manager, 3, p_user_name, p_is_logged_in, p_send_to_user);

            get_a_user_agreement_invitation = p_get_a_user_agreement_invitation;
            user_name = p_user_name;
            agreement_id = p_agreement_id;

            level_counts = 1;
            all_dialog_levels.Add(1, new DialogLevelsInformation(new ALevelOfDialog(FirstLevelFunction), DialogLevelType.SendingAMessage));
        }

        public void StartDialog()
        {
            Start();
        }

        private void FirstLevelFunction()
        {
            AgreementInvitationInfo user_agreement_invitation = null;

            int h = 0;
            while (h < 3)
            {
                try
                {
                    user_agreement_invitation = get_a_user_agreement_invitation(user_name,agreement_id);
                    h = 0;
                    break;
                }
                catch
                {
                    h++;
                    continue;
                }
            }

            if (h > 2)
            {
                End();
                return;
            }
            if (user_agreement_invitation == null)
            {
                End();
                return;
            }

            Di_Mess_InformInviteToAgreementInfo inform_invite_to_agreement_info = new Di_Mess_InformInviteToAgreementInfo(user_agreement_invitation);
            Send(TypeOfDialogMessage.InformInviteToAgreementInfo, inform_invite_to_agreement_info);
            return;
        }

        public override bool Equals(object obj)
        {
            Di_Server_ClientInvitedAgreementInform temp_dialog = null;
            try
            {
                temp_dialog = (Di_Server_ClientInvitedAgreementInform)obj;
            }
            catch
            {
                return false;
            }

            if (user_name == temp_dialog.user_name && agreement_id == temp_dialog.agreement_id)
            {
                return true;
            }
            return false;
        }
    }

    public abstract class BaseDialogManager
    {
        protected TypeOfDialog dialog_type;

        protected void BaseConstruct(TypeOfDialog p_dialog_type)
        {
            dialog_type = p_dialog_type;
        }

        public void ReceiveMessage(BaseServerDialogMessage p_message)
        {
            if (CheckInputMessage(p_message))
            {
                InitialProcessMessage(p_message);
            }

        }
        protected bool CheckInputMessage(BaseServerDialogMessage p_message)
        {
            return (HelperFunctions.CheckBaseServerDialogMessage(p_message) || HelperFunctions.DialogMessageObjectInvestigate(p_message.Get_message));
        }
        protected abstract void InitialProcessMessage(BaseServerDialogMessage p_message);

        public abstract void DialogRemoveItselfRequest(int dialog_id);
        public abstract void RemoveADilaog(int p_dialog_id);

    }

    public abstract class AuthDialogManager : BaseDialogManager
    {
        protected Dictionary<int, Se_AuthDialog> all_dialogs;

        protected AuthSend send_to_user;
        protected IsLoggedIn is_logged_in;

        protected void BaseAuthConstruct(TypeOfDialog p_dialog_type, ref Dictionary<int, Se_AuthDialog> p_dialogs, AuthSend p_send_to_user, IsLoggedIn p_is_logged_in)
        {
            base.BaseConstruct(p_dialog_type);
            send_to_user = p_send_to_user;
            is_logged_in = p_is_logged_in;
            all_dialogs = p_dialogs;
        }

        public override void DialogRemoveItselfRequest(int p_dialog_id)
        {
            all_dialogs.Remove(p_dialog_id);
        }

        public override void RemoveADilaog(int p_dialog_id)
        {
            if (all_dialogs.ContainsKey(p_dialog_id))
            {
                all_dialogs[p_dialog_id].ManagerRemoveRequestPath();
            }
        }

        public void RemoveUserDialog(string p_user_name)
        {
            for (int i = 0; i < all_dialogs.Count; i++)
            {
                if (all_dialogs.ElementAt(i).Value.Get_user_name == p_user_name)
                {
                    int temp_key = all_dialogs.ElementAt(i).Key;
                    all_dialogs[temp_key].ManagerRemoveRequestPath();
                    return;
                }
            }
            return;
        }

        protected override void InitialProcessMessage(BaseServerDialogMessage p_message)
        {
            if (p_message is AuthServerDialogMessage && p_message.Get_message.Get_dialog_type == dialog_type)
            {
                SecondaryProcessMessage((AuthServerDialogMessage)p_message);
            }
        }

        protected bool UserHasDialog(string p_user_name)
        {
            for (int i = 0; i < all_dialogs.Count; i++)
            {
                if (all_dialogs.ElementAt(i).Value.Get_user_name == p_user_name)
                {
                    return true;
                }
            }
            return false;
        }

        protected abstract void SecondaryProcessMessage(AuthServerDialogMessage p_message);

        protected bool Ma_IsLoggedIn(string p_user_name)
        {
            return is_logged_in(p_user_name);
        }
        protected void Ma_Authsend(string p_user_name, DialogMessageForClient p_message)
        {
            send_to_user(p_user_name, p_message);
        }

    }
    public abstract class UnAuthDialogManager : BaseDialogManager
    {
        protected Dictionary<int, Se_UnAuthDialog> all_dialogs;

        protected UnAuthSend send_to_usrer;
        protected IsThereUnauthWorkerThread is_there_unauththread;

        protected void BaseUnAuthConstruct(TypeOfDialog p_dialog_type, ref Dictionary<int, Se_UnAuthDialog> p_all_dialogs, UnAuthSend p_send_to_user
            , IsThereUnauthWorkerThread p_is_there_unauththread)
        {
            base.BaseConstruct(p_dialog_type);
            all_dialogs = p_all_dialogs;
            send_to_usrer = p_send_to_user;
            is_there_unauththread = p_is_there_unauththread;
        }

        public void RemoveUserDialog(int p_thread_id)
        {
            for (int i = 0; i < all_dialogs.Count; i++)
            {
                if (all_dialogs.ElementAt(i).Value.Get_thread_id == p_thread_id)
                {
                    int temp_key = all_dialogs.ElementAt(i).Key;
                    all_dialogs[temp_key].ManagerRemoveRequestPath();
                    return;
                }
            }
            return;
        }

        protected override void InitialProcessMessage(BaseServerDialogMessage p_message)
        {
            if (p_message is UnAuthServerDialogMessage && p_message.Get_message.Get_dialog_type == dialog_type)
            {
                SecondaryProcessMessage((UnAuthServerDialogMessage)p_message);
            }
        }

        protected abstract void SecondaryProcessMessage(UnAuthServerDialogMessage p_message);

        protected bool ThreadHasDialog(int p_thread_id)
        {
            for (int i = 0; i < all_dialogs.Count; i++)
            {
                if (all_dialogs.ElementAt(i).Value.Get_thread_id == p_thread_id)
                {
                    return true;
                }
            }
            return false;
        }

        public override void DialogRemoveItselfRequest(int p_dialog_id)
        {
            all_dialogs.Remove(p_dialog_id);
        }

        public override void RemoveADilaog(int p_dialog_id)
        {
            if (all_dialogs.ContainsKey(p_dialog_id))
            {
                all_dialogs[p_dialog_id].ManagerRemoveRequestPath();
            }
        }

        protected void Ma_UnAuthSend(int p_thread_id, DialogMessageForClient p_message)
        {
            send_to_usrer(p_thread_id, p_message);
        }
        protected bool Ma_IsThereUnauthWorkerThread(int p_thread_id)
        {
            return is_there_unauththread(p_thread_id);
        }

    }

    public abstract class CsAuthDialogManager : AuthDialogManager
    {
        protected abstract Se_AuthDialog CreateDialog(AuthServerDialogMessage p_dialog_start_message);

        protected override void SecondaryProcessMessage(AuthServerDialogMessage p_message)
        {
            if (is_logged_in(p_message.Get_user_name))
            {
                if (p_message.Get_message.Get_message_number_in_dialog == 1)
                {
                    Se_AuthDialog new_dialog = CreateDialog(p_message);
                    if (!all_dialogs.ContainsValue(new_dialog))
                    {
                        all_dialogs.Add(new_dialog.Get_dialog_id, new_dialog);
                    }
                }
                if (all_dialogs.ContainsKey(p_message.Get_message.Get_dialog_id))
                {
                    if (all_dialogs[p_message.Get_message.Get_dialog_id].Get_user_name == p_message.Get_user_name)
                    {
                        all_dialogs[p_message.Get_message.Get_dialog_id].ReceiveMessage(p_message.Get_message);
                    }
                }
            }
        }

    }
    public abstract class SsAuthDialogManager : AuthDialogManager
    {
        protected override void SecondaryProcessMessage(AuthServerDialogMessage p_message)
        {
            if (is_logged_in(p_message.Get_user_name))
            {
                if (all_dialogs.ContainsKey(p_message.Get_message.Get_dialog_id))
                {
                    if (all_dialogs[p_message.Get_message.Get_dialog_id].Get_user_name == p_message.Get_user_name)
                    {
                        all_dialogs[p_message.Get_message.Get_dialog_id].ReceiveMessage(p_message.Get_message);
                    }
                }
            }
        }
    }
    public abstract class CsUnAuthDialogManager : UnAuthDialogManager
    {
        protected abstract Se_UnAuthDialog CreateDialog(UnAuthServerDialogMessage p_dialog_start_message);

        protected override void SecondaryProcessMessage(UnAuthServerDialogMessage p_message)
        {
            if (is_there_unauththread(p_message.Get_thread_id))
            {
                if (p_message.Get_message.Get_message_number_in_dialog == 1)
                {
                    Se_UnAuthDialog new_dialog = CreateDialog(p_message);
                    if (!all_dialogs.ContainsValue(new_dialog))
                    {
                        all_dialogs.Add(new_dialog.Get_dialog_id, new_dialog);
                    }
                }
                if (all_dialogs.ContainsKey(p_message.Get_message.Get_dialog_id))
                {
                    if (all_dialogs[p_message.Get_message.Get_dialog_id].Get_thread_id == p_message.Get_thread_id)
                    {
                        all_dialogs[p_message.Get_message.Get_dialog_id].ReceiveMessage(p_message.Get_message);
                    }
                }
            }
        }
    }

    public class Ma_LoginDataRequestDialogManager : CsAuthDialogManager
    {
        GetFriendsList get_user_friends_list;
        GetUserStatus get_user_status;
        GetPublicChatIds get_public_chat_ids;
        GetOfflineMessages get_all_user_offline_message;
        GetAllAgreementInvitation get_all_user_agreement_invitation;

        public Ma_LoginDataRequestDialogManager(ref Dictionary<int, Se_AuthDialog> p_all_dialogs, GetFriendsList p_get_user_friends_list, GetUserStatus p_get_user_status, GetPublicChatIds p_get_public_chat_ids,
            GetOfflineMessages p_get_all_user_offline_message, AuthSend p_send_to_user, IsLoggedIn p_is_logged_in, GetAllAgreementInvitation p_get_all_user_agreement_invitation)
        {
            BaseAuthConstruct(TypeOfDialog.LoginDataRequest, ref p_all_dialogs, p_send_to_user, p_is_logged_in);
            get_user_friends_list = p_get_user_friends_list;
            get_user_status = p_get_user_status;
            get_public_chat_ids = p_get_public_chat_ids;
            get_all_user_offline_message = p_get_all_user_offline_message;
            get_all_user_agreement_invitation = p_get_all_user_agreement_invitation;
        }

        protected override Se_AuthDialog CreateDialog(AuthServerDialogMessage p_dialog_start_message)
        {
            return new Di_Server_LoginDataRequest(p_dialog_start_message.Get_message.Get_dialog_id, p_dialog_start_message.Get_user_name
                , new AuthSend(Ma_Authsend), new GetFriendsList(Ma_GetUserfriendList), new GetUserStatus(Ma_GetUserStatus), new GetPublicChatIds(Ma_GetPublicChatIds)
                , new GetOfflineMessages(Ma_GetAllUserOfflineMessages), new GetAllAgreementInvitation(Ma_GetAllUserAgreementInvitation)
                , new Remove(DialogRemoveItselfRequest), new IsLoggedIn(Ma_IsLoggedIn));

        }

        public List<AgreementInvitationInfo> Ma_GetAllUserAgreementInvitation(string p_user_name)
        {
            return get_all_user_agreement_invitation(p_user_name);
        }
        public List<OfflineMessage> Ma_GetAllUserOfflineMessages(string p_user_name)
        {
            return get_all_user_offline_message(p_user_name);
        }
        public List<int> Ma_GetPublicChatIds()
        {
            return get_public_chat_ids();
        }
        public UserStatus Ma_GetUserStatus(string p_user_name)
        {
            return get_user_status(p_user_name);
        }
        public List<string> Ma_GetUserfriendList(string p_user_name)
        {
            return get_user_friends_list(p_user_name);
        }

    }

    public class Ma_LoginRequestDialogManager : CsUnAuthDialogManager
    {
        Login server_login;

        public Ma_LoginRequestDialogManager(ref Dictionary<int, Se_UnAuthDialog> p_dialogs, UnAuthSend p_send_to_user, IsThereUnauthWorkerThread p_is_there_unauththread
            , Login p_server_login)
        {
            base.BaseUnAuthConstruct(TypeOfDialog.LoginRequest, ref p_dialogs, p_send_to_user, p_is_there_unauththread);

            server_login = p_server_login;
        }

        protected override Se_UnAuthDialog CreateDialog(UnAuthServerDialogMessage p_dialog_start_message)
        {
            return new Di_Server_LoginRequest(p_dialog_start_message.Get_message.Get_dialog_id, p_dialog_start_message.Get_thread_id, new Remove(DialogRemoveItselfRequest),
                new Login(Ma_Login), new UnAuthSend(Ma_UnAuthSend), new IsThereUnauthWorkerThread(Ma_IsThereUnauthWorkerThread));
        }

        public Se_BaseBooleanFunctionResult Ma_Login(int p_thread_id, string p_user_name, string p_password)
        {
            return server_login(p_thread_id, p_user_name, p_password);
        }

    }

    public class Ma_CreateAddAgreementDialogManager : CsAuthDialogManager
    {
        CreateAddAgreement create_add_agreement;
        CreateOfflineMessage create_offline_message;

        public Ma_CreateAddAgreementDialogManager(ref Dictionary<int, Se_AuthDialog> p_dialogs_and_lock, AuthSend p_send_to_user, IsLoggedIn p_is_logged_in
            , CreateAddAgreement p_create_add_agreement, CreateOfflineMessage p_create_offline_message)
        {
            base.BaseAuthConstruct(TypeOfDialog.CreateAddAgreement, ref p_dialogs_and_lock, p_send_to_user, p_is_logged_in);

            create_add_agreement = p_create_add_agreement;
            create_offline_message = p_create_offline_message;
        }

        protected override Se_AuthDialog CreateDialog(AuthServerDialogMessage p_dialog_start_message)
        {
            return new Di_Server_CreateAddAgreement(p_dialog_start_message.Get_message.Get_dialog_id,
                p_dialog_start_message.Get_user_name, new AuthSend(Ma_Authsend), new Remove(DialogRemoveItselfRequest), new CreateAddAgreement(Ma_CreateAddAgreement),
                new CreateOfflineMessage(Ma_CreateOfflineMessage), new IsLoggedIn(Ma_IsLoggedIn));
        }

        public Se_BaseBooleanFunctionResult Ma_CreateAddAgreement(string p_starter_user_name, string p_invited_user_name)
        {
            return create_add_agreement(p_starter_user_name, p_invited_user_name);
        }
        public void Ma_CreateOfflineMessage(string p_sender_user_name, OfflineMessage p_message)
        {
            create_offline_message(p_sender_user_name, p_message);
        }
    }

    public class Ma_GetAgreementAnswerDialogManager : CsAuthDialogManager
    {
        GetAgreementAnswer get_agreement_answer;

        public Ma_GetAgreementAnswerDialogManager(ref Dictionary<int, Se_AuthDialog> p_dialogs, AuthSend p_send_to_user, IsLoggedIn p_is_logged_in
            , GetAgreementAnswer p_get_agreement_answer)
        {
            base.BaseAuthConstruct(TypeOfDialog.GetAgreementAnswer, ref p_dialogs, p_send_to_user, p_is_logged_in);

            get_agreement_answer = p_get_agreement_answer;
        }

        protected override Se_AuthDialog CreateDialog(AuthServerDialogMessage p_dialog_start_message)
        {
            return new Di_Server_GetAgreementAnswer(p_dialog_start_message.Get_message.Get_dialog_id
                , p_dialog_start_message.Get_user_name, new AuthSend(Ma_Authsend), new Remove(DialogRemoveItselfRequest), new GetAgreementAnswer(Ma_GetAgreementAnswer)
                , new IsLoggedIn(Ma_IsLoggedIn));
        }

        public void Ma_GetAgreementAnswer(string p_user_name, int p_agreement_id, bool p_answer, TypeOfAgreement p_agreement_type)
        {
            get_agreement_answer(p_user_name, p_agreement_id, p_answer, p_agreement_type);
        }
    }

    public class Ma_ClientLeaveChatRequestDialogManager : CsAuthDialogManager
    {
        ServerInformChatLeave server_inform_chat_leave;

        public Ma_ClientLeaveChatRequestDialogManager(ref Dictionary<int, Se_AuthDialog> p_all_dialogs, AuthSend p_send_to_user, IsLoggedIn p_is_logged_in,
            ServerInformChatLeave p_server_inform_chat_leave)
        {
            base.BaseAuthConstruct(TypeOfDialog.ClientLeaveChatRequest, ref p_all_dialogs, p_send_to_user, p_is_logged_in);

            server_inform_chat_leave = p_server_inform_chat_leave;
        }

        protected override Se_AuthDialog CreateDialog(AuthServerDialogMessage p_dialog_start_message)
        {
            return new Di_Server_ClientLeaveChatRequest(p_dialog_start_message.Get_message.Get_dialog_id,
                p_dialog_start_message.Get_user_name, new AuthSend(Ma_Authsend), new Remove(DialogRemoveItselfRequest), new ServerInformChatLeave(Ma_ServerInformChatLeave),
                new IsLoggedIn(Ma_IsLoggedIn));
        }

        public void Ma_ServerInformChatLeave(string p_user_name, int p_chat_id, TypeOfChat p_chat_type)
        {
            server_inform_chat_leave(p_user_name, p_chat_id, p_chat_type);
        }
    }

    public class Ma_ClientSomeoneLeftChatInformDialogManager : SsAuthDialogManager
    {
        public Ma_ClientSomeoneLeftChatInformDialogManager(ref Dictionary<int, Se_AuthDialog> p_dialogs_and_lock, AuthSend p_send_to_user, IsLoggedIn p_is_logged_in)
        {
            base.BaseAuthConstruct(TypeOfDialog.ClientSomeoneLeftChatInform, ref p_dialogs_and_lock, p_send_to_user, p_is_logged_in);
        }

        public void Create(string p_user_to_inform_name, string p_user_left_chat_name, int p_chat_id, TypeOfChat p_chat_type)
        {
            int random_dialog_id = HelperFunctions.GetGUID();
            if (is_logged_in(p_user_to_inform_name))
            {
                Di_Server_ClientSomeoneLeftChatInform temp_dialog = new Di_Server_ClientSomeoneLeftChatInform(random_dialog_id, p_user_to_inform_name
                    , new AuthSend(Ma_Authsend), new Remove(DialogRemoveItselfRequest), p_user_left_chat_name, p_chat_id, new IsLoggedIn(Ma_IsLoggedIn)
                    , p_chat_type);
                if (!all_dialogs.ContainsValue(temp_dialog))
                {
                    all_dialogs.Add(temp_dialog.Get_dialog_id, temp_dialog);
                    temp_dialog.StartDialog();
                }
            }
        }
    }

    public class Ma_InformEjectedChatUserDialogManager : SsAuthDialogManager
    {
        public Ma_InformEjectedChatUserDialogManager(ref Dictionary<int, Se_AuthDialog> p_dialogs_and_lock, AuthSend p_send_to_user, IsLoggedIn p_is_logged_in)
        {
            base.BaseAuthConstruct(TypeOfDialog.InformEjectedChatUser, ref p_dialogs_and_lock, p_send_to_user, p_is_logged_in);
        }

        public void Create(string p_user_name, int p_chat_id_user_ejected_from, string p_ejecting_comment, TypeOfChat p_chat_type)
        {
            int random_dialog_id = HelperFunctions.GetGUID();
            if (is_logged_in(p_user_name))
            {
                Di_server_InformEjectedChatUser temp_dialog = new Di_server_InformEjectedChatUser(random_dialog_id, p_user_name, new AuthSend(Ma_Authsend), new Remove(DialogRemoveItselfRequest)
                , new IsLoggedIn(Ma_IsLoggedIn), p_chat_id_user_ejected_from, p_ejecting_comment, p_chat_type);
                if (!all_dialogs.ContainsValue(temp_dialog))
                {
                    all_dialogs.Add(temp_dialog.Get_dialog_id, temp_dialog);
                    temp_dialog.StartDialog();
                }
            }
        }
    }

    public class Ma_CreatePrivateChatRequestDialogManager : CsAuthDialogManager
    {
        OnlineAreFriends are_friends;
        CreatePrivateChat create_private_chat;
        IsTherePrivateChat is_there_private_chat;
        StartClientCreatedPrivateChatInform start_client_created_private_chat_inform;

        public Ma_CreatePrivateChatRequestDialogManager(ref Dictionary<int, Se_AuthDialog> p_dialogs_and_lock, AuthSend p_send_to_user, IsLoggedIn p_is_logged_in
            , OnlineAreFriends p_are_friends, CreatePrivateChat p_create_private_chat, IsTherePrivateChat p_is_there_private_chat
            , StartClientCreatedPrivateChatInform p_start_client_created_private_chat_inform)
        {
            base.BaseAuthConstruct(TypeOfDialog.CreatePrivateChatRequest, ref p_dialogs_and_lock, p_send_to_user, p_is_logged_in);

            are_friends = p_are_friends;
            create_private_chat = p_create_private_chat;
            is_there_private_chat = p_is_there_private_chat;
            start_client_created_private_chat_inform = p_start_client_created_private_chat_inform;
        }

        protected override Se_AuthDialog CreateDialog(AuthServerDialogMessage p_dialog_start_message)
        {
            return new Di_Server_CreatePrivateChatRequest(p_dialog_start_message.Get_message.Get_dialog_id, p_dialog_start_message.Get_user_name,
                  new AuthSend(Ma_Authsend), new Remove(DialogRemoveItselfRequest), new IsLoggedIn(Ma_IsLoggedIn), new OnlineAreFriends(Ma_AreFriends),
                  new CreatePrivateChat(Ma_CreatePrivateChat), new StartClientCreatedPrivateChatInform(Ma_StartClientCreatedPrivateChatInform),
                  new IsTherePrivateChat(Ma_IsTherePrivateChat));

        }

        public Se_BaseBooleanFunctionResult Ma_AreFriends(string p_first_person, string p_second_person)
        {
            return are_friends(p_first_person, p_second_person);
        }
        public Se_BaseIntFunctionResult Ma_CreatePrivateChat(string p_first_person, string p_second_person)
        {
            return create_private_chat(p_first_person, p_second_person);
        }
        public bool Ma_IsTherePrivateChat(int p_chat_id)
        {
            return is_there_private_chat(p_chat_id);
        }
        public void Ma_StartClientCreatedPrivateChatInform(string p_user_name, string p_starter_user_name, int p_chat_id)
        {
            start_client_created_private_chat_inform(p_user_name, p_starter_user_name, p_chat_id);
        }

    }

    public class Ma_ClientJoinPublicChatRequestDialogManager : CsAuthDialogManager
    {
        JoinPublicChatRequest join_public_chat_request;
        IsUserInPublicChat is_user_in_public_chat;
        GetPublicChatUsersList get_public_chat_users_list;

        public Ma_ClientJoinPublicChatRequestDialogManager(ref Dictionary<int, Se_AuthDialog> p_dialogs, AuthSend p_send_to_user, IsLoggedIn p_is_logged_in
            , JoinPublicChatRequest p_join_public_chat_request, IsUserInPublicChat p_is_user_in_public_chat, GetPublicChatUsersList p_get_public_chat_users_list)
        {
            base.BaseAuthConstruct(TypeOfDialog.ClientJoinPublicChatRequest, ref p_dialogs, p_send_to_user, p_is_logged_in);

            join_public_chat_request = p_join_public_chat_request;
            is_user_in_public_chat = p_is_user_in_public_chat;
            get_public_chat_users_list = p_get_public_chat_users_list;
        }

        protected override Se_AuthDialog CreateDialog(AuthServerDialogMessage p_dialog_start_message)
        {
            return new Di_Server_ClientJoinPublicChatRequest(p_dialog_start_message.Get_message.Get_dialog_id
                , p_dialog_start_message.Get_user_name, new AuthSend(Ma_Authsend), new Remove(DialogRemoveItselfRequest), new JoinPublicChatRequest(Ma_JoinPublicChatRequest),
                new IsLoggedIn(Ma_IsLoggedIn), new IsUserInPublicChat(Ma_IsUserInPublicChat), new GetPublicChatUsersList(Ma_GetPublicChatUsersList));
        }

        public Se_BaseBooleanFunctionResult Ma_JoinPublicChatRequest(string p_user_name, int p_chat_id)
        {
            return join_public_chat_request(p_user_name, p_chat_id);
        }
        public bool Ma_IsUserInPublicChat(string p_user_name, int p_chat_id)
        {
            return is_user_in_public_chat(p_user_name, p_chat_id);
        }
        public List<string> Ma_GetPublicChatUsersList(int p_chat_id)
        {
            return get_public_chat_users_list(p_chat_id);
        }
    }

    public class Ma_ClientSomeoneJoinedChatInformDialogManager : SsAuthDialogManager
    {
        public Ma_ClientSomeoneJoinedChatInformDialogManager(ref Dictionary<int, Se_AuthDialog> p_dialogs, AuthSend p_send_to_user, IsLoggedIn p_is_logged_in)
        {
            base.BaseAuthConstruct(TypeOfDialog.ClientSomeoneJoinedChatInform, ref p_dialogs, p_send_to_user, p_is_logged_in);
        }

        public void Create(string p_user_to_inform_name, string p_joined_user_name, int p_chat_id, TypeOfChat p_chat_type)
        {
            int random_dialog_id = HelperFunctions.GetGUID();
            if (is_logged_in(p_user_to_inform_name))
            {
                Di_Server_ClientSomeoneJoinedChatInform temp_dialog = new Di_Server_ClientSomeoneJoinedChatInform(random_dialog_id, p_user_to_inform_name, new AuthSend(Ma_Authsend)
                , new Remove(DialogRemoveItselfRequest), new IsLoggedIn(Ma_IsLoggedIn), p_joined_user_name, p_chat_id, p_chat_type);
                if (!all_dialogs.ContainsValue(temp_dialog))
                {
                    all_dialogs.Add(temp_dialog.Get_dialog_id, temp_dialog);
                    temp_dialog.StartDialog();
                }
            }
        }
    }

    public class Ma_FormalMessageRequestDialogManager : CsAuthDialogManager
    {
        CreateFormalMessageRequest send_fromal_message_to_user;
        IstherUser user_exist;
        CreateOfflineMessage create_offline_message;

        public Ma_FormalMessageRequestDialogManager(ref Dictionary<int, Se_AuthDialog> p_dialogs, AuthSend p_send_to_user, IsLoggedIn p_is_logged_in
            , CreateFormalMessageRequest p_send_fromal_message_to_user, IstherUser p_user_exist, CreateOfflineMessage p_create_offline_message)
        {
            base.BaseAuthConstruct(TypeOfDialog.FormalMessageRequest, ref p_dialogs, p_send_to_user, p_is_logged_in);

            send_fromal_message_to_user = p_send_fromal_message_to_user;
            user_exist = p_user_exist;
            create_offline_message = p_create_offline_message;
        }

        protected override Se_AuthDialog CreateDialog(AuthServerDialogMessage p_dialog_start_message)
        {
            return new Di_Server_FormalMessageRequest(p_dialog_start_message.Get_message.Get_dialog_id,
                p_dialog_start_message.Get_user_name, new AuthSend(Ma_Authsend), new Remove(DialogRemoveItselfRequest), new CreateFormalMessageRequest(Ma_SendFormalMessageToUser),
                new IstherUser(Ma_IstherUser), new IsLoggedIn(Ma_IsLoggedIn), new CreateOfflineMessage(Ma_CreateOfflineMessage));
        }

        public void Ma_SendFormalMessageToUser(FormalMessage p_message, string p_receiver_user_name)
        {
            send_fromal_message_to_user(p_message, p_receiver_user_name);
        }
        public bool Ma_IstherUser(string p_user_name)
        {
            return user_exist(p_user_name);
        }
        public void Ma_CreateOfflineMessage(string p_receiver_user_name, OfflineMessage p_message)
        {
            create_offline_message(p_receiver_user_name, p_message);
        }
    }

    public class Ma_ClientSignupRequestDialogManager : CsUnAuthDialogManager
    {
        SignUp signup_request;

        public Ma_ClientSignupRequestDialogManager(ref Dictionary<int, Se_UnAuthDialog> p_all_dialogs_and_locks, UnAuthSend p_send_to_user
            , IsThereUnauthWorkerThread p_is_there_unauththread, SignUp p_signup_request)
        {
            base.BaseUnAuthConstruct(TypeOfDialog.ClientSignupRequest, ref p_all_dialogs_and_locks, p_send_to_user, p_is_there_unauththread);

            signup_request = p_signup_request;
        }

        protected override Se_UnAuthDialog CreateDialog(UnAuthServerDialogMessage p_dialog_start_message)
        {
            return new Di_Server_ClientSignupRequest(p_dialog_start_message.Get_message.Get_dialog_id, p_dialog_start_message.Get_thread_id
                , new UnAuthSend(Ma_UnAuthSend), new Remove(DialogRemoveItselfRequest), new SignUp(Ma_Signup), new IsThereUnauthWorkerThread(Ma_IsThereUnauthWorkerThread));
        }

        public Se_BaseBooleanFunctionResult Ma_Signup(string p_user_name, string p_password)
        {
            return signup_request(p_user_name, p_password);
        }
    }

    public class Ma_ClientFriendChangedStatusInformDialogManager : SsAuthDialogManager
    {
        public Ma_ClientFriendChangedStatusInformDialogManager(ref Dictionary<int, Se_AuthDialog> p_dialogs, AuthSend p_send_to_user, IsLoggedIn p_is_logged_in)
        {
            base.BaseAuthConstruct(TypeOfDialog.ClientFriendChangedStatusInform, ref p_dialogs, p_send_to_user, p_is_logged_in);
        }

        public void Create(string p_user_to_inform_name, string p_user_changed_status_name, UserStatus p_new_status)
        {
            int random_dialog_id = HelperFunctions.GetGUID();
            if (is_logged_in(p_user_to_inform_name))
            {
                Di_Server_ClientFriendChangedStatusInform temp_dialog = new Di_Server_ClientFriendChangedStatusInform(random_dialog_id, p_user_to_inform_name, new Remove(DialogRemoveItselfRequest)
                , new IsLoggedIn(Ma_IsLoggedIn), new AuthSend(Ma_Authsend), p_user_changed_status_name, p_new_status);
                if (!all_dialogs.ContainsValue(temp_dialog))
                {
                    all_dialogs.Add(temp_dialog.Get_dialog_id, temp_dialog);
                    temp_dialog.StartDialog();
                }
            }
        }
    }

    public class Ma_SendToClinetFormalMessageDialogManager : SsAuthDialogManager
    {
        public Ma_SendToClinetFormalMessageDialogManager(ref Dictionary<int, Se_AuthDialog> p_dialogs, AuthSend p_send_to_user, IsLoggedIn p_is_logged_in)
        {
            base.BaseAuthConstruct(TypeOfDialog.SendToClinetFormalMessage, ref p_dialogs, p_send_to_user, p_is_logged_in);
        }

        public void Create(string p_receiver_user_name, string p_sender_user_name, string p_message_text, int p_message_id)
        {
            int random_dialog_id = HelperFunctions.GetGUID();
            if (is_logged_in(p_receiver_user_name))
            {
                Di_Server_SendToClinetFormalMessage temp_dialog = new Di_Server_SendToClinetFormalMessage(random_dialog_id, new Remove(DialogRemoveItselfRequest)
                , p_receiver_user_name, new IsLoggedIn(Ma_IsLoggedIn), new AuthSend(Ma_Authsend), p_sender_user_name, p_message_text, p_message_id);
                if (!all_dialogs.ContainsValue(temp_dialog))
                {
                    all_dialogs.Add(temp_dialog.Get_dialog_id, temp_dialog);
                    temp_dialog.StartDialog();
                }
            }
        }
    }

    public class Ma_ServerReadOfflineMessagesInformDialogManager : CsAuthDialogManager
    {
        OfflineMessagesReadInform offline_message_read_inform;

        public Ma_ServerReadOfflineMessagesInformDialogManager(ref Dictionary<int, Se_AuthDialog> p_dialogs, AuthSend p_send_to_user, IsLoggedIn p_is_logged_in
            , OfflineMessagesReadInform p_offline_message_read_inform)
        {
            base.BaseAuthConstruct(TypeOfDialog.ServerReadOfflineMessagesInform, ref p_dialogs, p_send_to_user, p_is_logged_in);

            offline_message_read_inform = p_offline_message_read_inform;
        }

        protected override Se_AuthDialog CreateDialog(AuthServerDialogMessage p_dialog_start_message)
        {
            return new Di_Server_ServerReadOfflineMessagesInform(p_dialog_start_message.Get_message.Get_dialog_id
                , new Remove(DialogRemoveItselfRequest), p_dialog_start_message.Get_user_name, new IsLoggedIn(Ma_IsLoggedIn), new AuthSend(Ma_Authsend)
                , new OfflineMessagesReadInform(Ma_OfflineMessagesReadInform));
        }

        public void Ma_OfflineMessagesReadInform(string p_user_name, List<int> p_message_ids)
        {
            offline_message_read_inform(p_user_name, p_message_ids);
        }

    }

    public class Ma_ClientFriendListChangedInformdialogManager : SsAuthDialogManager
    {
        public Ma_ClientFriendListChangedInformdialogManager(ref Dictionary<int, Se_AuthDialog> p_dialogs, AuthSend p_send_to_user, IsLoggedIn p_is_logged_in)
        {
            base.BaseAuthConstruct(TypeOfDialog.ClientFriendListChangedInform, ref p_dialogs, p_send_to_user, p_is_logged_in);
        }

        public void Create(string p_user_name, List<PersonStatus> p_new_friend_list_and_status)
        {
            int random_dialog_id = HelperFunctions.GetGUID();
            if (is_logged_in(p_user_name))
            {
                Di_Server_ClientFriendListChangedInform temp_dialog = new Di_Server_ClientFriendListChangedInform(random_dialog_id, new Remove(DialogRemoveItselfRequest)
                    , new IsLoggedIn(Ma_IsLoggedIn), new AuthSend(Ma_Authsend), p_user_name, p_new_friend_list_and_status);
                if (!all_dialogs.ContainsValue(temp_dialog))
                {
                    all_dialogs.Add(temp_dialog.Get_dialog_id, temp_dialog);
                    temp_dialog.StartDialog();
                }
            }
        }
    }

    public class Ma_ClientCreatedPrivateChatInformDialogManager : SsAuthDialogManager
    {
        GetPrivateChatInvitationAnswer get_private_chat_invitation_answer;
        IsTherePrivateChat is_there_private_chat;

        public Ma_ClientCreatedPrivateChatInformDialogManager(ref Dictionary<int, Se_AuthDialog> p_dialogs, AuthSend p_send_to_user, IsLoggedIn p_is_logged_in
            , GetPrivateChatInvitationAnswer p_get_private_chat_invitation_answer, IsTherePrivateChat p_is_there_private_chat)
        {
            base.BaseAuthConstruct(TypeOfDialog.ClientCreatedPrivateChatInform, ref p_dialogs, p_send_to_user, p_is_logged_in);

            get_private_chat_invitation_answer = p_get_private_chat_invitation_answer;
            is_there_private_chat = p_is_there_private_chat;
        }

        public void Create(string p_user_name, string p_starter_user_name, int p_chat_id)
        {
            int random_dialog_id = HelperFunctions.GetGUID();
            if (is_logged_in(p_user_name))
            {
                Di_Server_ClientCreatedPrivateChatInform temp_dialog = new Di_Server_ClientCreatedPrivateChatInform(random_dialog_id
                    , new Remove(DialogRemoveItselfRequest), new IsLoggedIn(Ma_IsLoggedIn), new AuthSend(Ma_Authsend), p_user_name
                    , p_starter_user_name, p_chat_id, new IsTherePrivateChat(Ma_IsTherePrivateChat), new GetPrivateChatInvitationAnswer(Ma_GetPrivateChatInvitationAnswer));
                if (!all_dialogs.ContainsValue(temp_dialog))
                {
                    all_dialogs.Add(temp_dialog.Get_dialog_id, temp_dialog);
                    temp_dialog.StartDialog();
                }
            }
        }

        public Se_BaseBooleanFunctionResult Ma_GetPrivateChatInvitationAnswer(string p_user_name, int p_chat_id, bool p_answer)
        {
            return get_private_chat_invitation_answer(p_user_name, p_chat_id, p_answer);
        }
        public bool Ma_IsTherePrivateChat(int p_chat_id)
        {
            return is_there_private_chat(p_chat_id);
        }
    }

    public class Ma_ClientInvitedAgreementInformDialogManager : SsAuthDialogManager
    {
        GetAUserAgreementInvitation get_a_user_agreement_invitation;

        public Ma_ClientInvitedAgreementInformDialogManager(ref Dictionary<int, Se_AuthDialog> p_dialogs, AuthSend p_send_to_user, IsLoggedIn p_is_logged_in
            , GetAUserAgreementInvitation p_get_a_user_agreement_invitation)
        {
            base.BaseAuthConstruct(TypeOfDialog.ClientInvitedAgreementInform, ref p_dialogs, p_send_to_user, p_is_logged_in);
            get_a_user_agreement_invitation = p_get_a_user_agreement_invitation;
        }

        public void Create(string p_user_name, int p_agreement_id)
        {
            int random_dialog_id = HelperFunctions.GetGUID();
            if (is_logged_in(p_user_name))
            {
                Di_Server_ClientInvitedAgreementInform temp_dialog = new Di_Server_ClientInvitedAgreementInform(random_dialog_id, new Remove(DialogRemoveItselfRequest)
                , new IsLoggedIn(Ma_IsLoggedIn), new AuthSend(Ma_Authsend), p_user_name, p_agreement_id, new GetAUserAgreementInvitation(Ma_GetAUserAgreementInvitation));
                if (!all_dialogs.ContainsValue(temp_dialog))
                {
                    all_dialogs.Add(temp_dialog.Get_dialog_id, temp_dialog);
                    temp_dialog.StartDialog();
                }
            }
        }

        public AgreementInvitationInfo Ma_GetAUserAgreementInvitation(string p_user_name, int p_agreement_id)
        {
            return get_a_user_agreement_invitation(p_user_name, p_agreement_id);
        }
    }


    public class AllDialogs
    {
        GetFriendsList get_user_friends_list;
        GetUserStatus get_user_status;
        GetPublicChatIds get_public_chat_ids;
        GetOfflineMessages get_all_user_offline_message;
        GetAllAgreementInvitation get_all_user_agreement_invitation;
        AuthSend auth_send_to_user;
        IsLoggedIn is_logged_in;
        UnAuthSend unauth_send_to_usrer;
        IsThereUnauthWorkerThread is_there_unauththread;
        Login server_login;
        ServerInformChatLeave server_inform_chat_leave;
        OnlineAreFriends are_friends;
        CreatePrivateChat create_private_chat;
        IsTherePrivateChat is_there_private_chat;
        StartClientCreatedPrivateChatInform start_client_created_private_chat_inform;
        JoinPublicChatRequest join_public_chat_request;
        IsUserInPublicChat is_user_in_public_chat;
        GetPublicChatUsersList get_public_chat_users_list;
        CreateFormalMessageRequest send_fromal_message_to_user;
        IstherUser user_exist;
        CreateOfflineMessage create_offline_message;
        SignUp signup_request;
        CreateAddAgreement create_add_agreement;
        GetAgreementAnswer get_agreement_answer;
        OfflineMessagesReadInform offline_message_read_inform;
        GetPrivateChatInvitationAnswer get_private_chat_invitation_answer;
        GetAUserAgreementInvitation get_a_user_agreement_invitation;

        Ma_LoginDataRequestDialogManager ma_login_request_data_dialog_manager;
        Ma_LoginRequestDialogManager ma_login_request_dialog_manager;
        Ma_ClientLeaveChatRequestDialogManager ma_client_leave_chat_request_dialog_manager;
        Ma_ClientSomeoneLeftChatInformDialogManager ma_client_someone_left_chat_inform_dialog_manager;
        Ma_InformEjectedChatUserDialogManager ma_inform_ejected_chat_user_dialog_manager;
        Ma_CreatePrivateChatRequestDialogManager ma_create_private_chat_request_dialog_manager;
        Ma_ClientJoinPublicChatRequestDialogManager ma_client_join_public_chat_request_dialog_manager;
        Ma_ClientSomeoneJoinedChatInformDialogManager ma_client_someone_jioned_chat_inform_dialog_manager;
        Ma_FormalMessageRequestDialogManager ma_formal_message_request_dialog_manager;
        Ma_ClientSignupRequestDialogManager ma_client_signup_request_dialog_manager;
        Ma_CreateAddAgreementDialogManager ma_create_add_agreement_dialog_manager;
        Ma_GetAgreementAnswerDialogManager ma_get_agreement_answer_dialog_manager;
        Ma_ClientFriendChangedStatusInformDialogManager ma_client_friend_changed_status_inform_dialog_manager;
        Ma_SendToClinetFormalMessageDialogManager ma_send_to_client_formal_message_dialog_manager;
        Ma_ServerReadOfflineMessagesInformDialogManager ma_server_read_offline_messages_inform_dialog_manager;
        Ma_ClientFriendListChangedInformdialogManager ma_client_friend_list_changed_inform_dialog_manager;
        Ma_ClientCreatedPrivateChatInformDialogManager ma_client_created_private_chat_inform_dialog_manager;
        Ma_ClientInvitedAgreementInformDialogManager ma_client_invited_agreement_inform_dialog_manager;


        public AllDialogs(Se_ServerDelegateForDialogs p_server_delegate_for_dialogs, ref Dictionary<TypeOfDialog, Dictionary<int, Se_AuthDialog>> p_auth_dialogs
            , ref Dictionary<TypeOfDialog, Dictionary<int, Se_UnAuthDialog>> p_unauth_dialogs)
        {
            get_user_friends_list = p_server_delegate_for_dialogs.get_user_friends_list;
            get_user_status = p_server_delegate_for_dialogs.get_user_status;
            get_public_chat_ids = p_server_delegate_for_dialogs.get_public_chat_ids;
            get_all_user_offline_message = p_server_delegate_for_dialogs.get_all_user_offline_message;
            get_all_user_agreement_invitation = p_server_delegate_for_dialogs.get_all_user_agreement_invitation;
            auth_send_to_user = p_server_delegate_for_dialogs.auth_send_to_user;
            is_logged_in = p_server_delegate_for_dialogs.is_logged_in;
            unauth_send_to_usrer = p_server_delegate_for_dialogs.unauth_send_to_usrer;
            is_there_unauththread = p_server_delegate_for_dialogs.is_there_unauththread;
            server_login = p_server_delegate_for_dialogs.server_login;
            server_inform_chat_leave = p_server_delegate_for_dialogs.server_inform_chat_leave;
            are_friends = p_server_delegate_for_dialogs.are_friends;
            create_private_chat = p_server_delegate_for_dialogs.create_private_chat;
            is_there_private_chat = p_server_delegate_for_dialogs.is_there_private_chat;
            start_client_created_private_chat_inform = p_server_delegate_for_dialogs.start_client_created_private_chat_inform;
            join_public_chat_request = p_server_delegate_for_dialogs.join_public_chat_request;
            is_user_in_public_chat = p_server_delegate_for_dialogs.is_user_in_public_chat;
            get_public_chat_users_list = p_server_delegate_for_dialogs.get_public_chat_users_list;
            send_fromal_message_to_user = p_server_delegate_for_dialogs.send_fromal_message_to_user;
            user_exist = p_server_delegate_for_dialogs.user_exist;
            create_offline_message = p_server_delegate_for_dialogs.create_offline_message;
            signup_request = p_server_delegate_for_dialogs.signup_request;
            create_add_agreement = p_server_delegate_for_dialogs.create_add_agreement;
            get_agreement_answer = p_server_delegate_for_dialogs.get_agreement_answer;
            offline_message_read_inform = p_server_delegate_for_dialogs.offline_message_read_inform;
            get_private_chat_invitation_answer = p_server_delegate_for_dialogs.get_private_chat_invitation_answer;
            get_a_user_agreement_invitation = p_server_delegate_for_dialogs.get_a_user_agreement_invitation;

            Dictionary<int, Se_AuthDialog> ma_login_request_data_dialog_manager_users_list;
            Dictionary<int, Se_AuthDialog> ma_client_leave_chat_request_dialog_manager_users_list;
            Dictionary<int, Se_AuthDialog> ma_client_someone_left_chat_inform_dialog_manager_users_list;
            Dictionary<int, Se_AuthDialog> ma_inform_ejected_chat_user_dialog_manager_users_list;
            Dictionary<int, Se_AuthDialog> ma_create_private_chat_request_dialog_manager_users_list;
            Dictionary<int, Se_AuthDialog> ma_client_join_public_chat_request_dialog_manager_users_list;
            Dictionary<int, Se_AuthDialog> ma_client_someone_jioned_chat_inform_dialog_manager_users_list;
            Dictionary<int, Se_AuthDialog> ma_formal_message_request_dialog_manager_users_list;
            Dictionary<int, Se_AuthDialog> ma_create_add_agreement_dialog_manager_users_list;
            Dictionary<int, Se_AuthDialog> ma_get_agreement_answer_dialog_manager_users_list;
            Dictionary<int, Se_AuthDialog> ma_client_friend_changed_status_inform_dialog_manager_users_list;
            Dictionary<int, Se_UnAuthDialog> ma_client_signup_request_dialog_manager_users_list;
            Dictionary<int, Se_UnAuthDialog> ma_login_request_dialog_manager_users_list; 
            Dictionary<int, Se_AuthDialog> ma_send_to_client_formal_message_dialog_manager_users_list;
            Dictionary<int, Se_AuthDialog> ma_server_read_offline_messages_inform_dialog_manager_users_list;
            Dictionary<int, Se_AuthDialog> ma_client_friend_list_changed_inform_dialog_manager_users_list;
            Dictionary<int, Se_AuthDialog> ma_client_created_private_chat_inform_dialog_manager_users_list;
            Dictionary<int, Se_AuthDialog>  ma_client_invited_agreement_inform_dialog_manager_users_list;

            ma_login_request_data_dialog_manager_users_list = p_auth_dialogs[TypeOfDialog.LoginDataRequest];
            ma_client_leave_chat_request_dialog_manager_users_list = p_auth_dialogs[TypeOfDialog.ClientLeaveChatRequest];
            ma_client_someone_left_chat_inform_dialog_manager_users_list = p_auth_dialogs[TypeOfDialog.ClientSomeoneLeftChatInform];
            ma_inform_ejected_chat_user_dialog_manager_users_list = p_auth_dialogs[TypeOfDialog.InformEjectedChatUser];
            ma_create_private_chat_request_dialog_manager_users_list = p_auth_dialogs[TypeOfDialog.CreatePrivateChatRequest];
            ma_client_join_public_chat_request_dialog_manager_users_list = p_auth_dialogs[TypeOfDialog.ClientJoinPublicChatRequest];
            ma_client_someone_jioned_chat_inform_dialog_manager_users_list = p_auth_dialogs[TypeOfDialog.ClientSomeoneJoinedChatInform];
            ma_formal_message_request_dialog_manager_users_list = p_auth_dialogs[TypeOfDialog.FormalMessageRequest];
            ma_create_add_agreement_dialog_manager_users_list = p_auth_dialogs[TypeOfDialog.CreateAddAgreement];
            ma_get_agreement_answer_dialog_manager_users_list = p_auth_dialogs[TypeOfDialog.GetAgreementAnswer];
            ma_client_friend_changed_status_inform_dialog_manager_users_list = p_auth_dialogs[TypeOfDialog.ClientFriendChangedStatusInform];
            ma_client_signup_request_dialog_manager_users_list = p_unauth_dialogs[TypeOfDialog.ClientSignupRequest];
            ma_login_request_dialog_manager_users_list = p_unauth_dialogs[TypeOfDialog.LoginRequest];
            ma_send_to_client_formal_message_dialog_manager_users_list = p_auth_dialogs[TypeOfDialog.SendToClinetFormalMessage];
            ma_server_read_offline_messages_inform_dialog_manager_users_list = p_auth_dialogs[TypeOfDialog.ServerReadOfflineMessagesInform];
            ma_client_friend_list_changed_inform_dialog_manager_users_list = p_auth_dialogs[TypeOfDialog.ClientFriendListChangedInform];
            ma_client_created_private_chat_inform_dialog_manager_users_list = p_auth_dialogs[TypeOfDialog.ClientCreatedPrivateChatInform];
            ma_client_invited_agreement_inform_dialog_manager_users_list = p_auth_dialogs[TypeOfDialog.ClientInvitedAgreementInform];


            ma_login_request_data_dialog_manager = new Ma_LoginDataRequestDialogManager(ref ma_login_request_data_dialog_manager_users_list, new GetFriendsList(AllMa_GetUserfriendList)
            , new GetUserStatus(AllMa_GetUserStatus), new GetPublicChatIds(AllMa_GetPublicChatIds), new GetOfflineMessages(AllMa_GetAllUserOfflineMessages), new AuthSend(AllMa_Authsend)
            , new IsLoggedIn(AllMa_IsLoggedIn), new GetAllAgreementInvitation(AllMa_GetAllUserAgreementInvitation));

            ma_login_request_dialog_manager = new Ma_LoginRequestDialogManager(ref ma_login_request_dialog_manager_users_list, new UnAuthSend(AllMa_UnAuthSend)
            , new IsThereUnauthWorkerThread(AllMa_IsThereUnauthWorkerThread), new Login(AllMa_Login));

            ma_client_leave_chat_request_dialog_manager = new Ma_ClientLeaveChatRequestDialogManager(ref ma_client_leave_chat_request_dialog_manager_users_list,
                new AuthSend(AllMa_Authsend), new IsLoggedIn(AllMa_IsLoggedIn), new ServerInformChatLeave(AllMa_ServerInformChatLeave));

            ma_client_someone_left_chat_inform_dialog_manager = new Ma_ClientSomeoneLeftChatInformDialogManager(ref ma_client_someone_left_chat_inform_dialog_manager_users_list
                , new AuthSend(AllMa_Authsend), new IsLoggedIn(AllMa_IsLoggedIn));

            ma_inform_ejected_chat_user_dialog_manager = new Ma_InformEjectedChatUserDialogManager(ref ma_inform_ejected_chat_user_dialog_manager_users_list, new AuthSend(AllMa_Authsend)
            , new IsLoggedIn(AllMa_IsLoggedIn));

            ma_create_private_chat_request_dialog_manager = new Ma_CreatePrivateChatRequestDialogManager(ref ma_create_private_chat_request_dialog_manager_users_list,
                new AuthSend(AllMa_Authsend), new IsLoggedIn(AllMa_IsLoggedIn), new OnlineAreFriends(AllMa_AreFriends), new CreatePrivateChat(AllMa_CreatePrivateChat)
                , new IsTherePrivateChat(AllMa_IsTherePrivateChat), new StartClientCreatedPrivateChatInform(AllMa_startClientCreatedPrivateChatInform));

            ma_client_join_public_chat_request_dialog_manager = new Ma_ClientJoinPublicChatRequestDialogManager(ref ma_client_join_public_chat_request_dialog_manager_users_list,
                new AuthSend(AllMa_Authsend), new IsLoggedIn(AllMa_IsLoggedIn), new JoinPublicChatRequest(AllMa_JoinPublicChatRequest)
                , new IsUserInPublicChat(AllMa_IsUserInPublicChat), new GetPublicChatUsersList(AllMa_GetPublicChatUsersList));

            ma_client_someone_jioned_chat_inform_dialog_manager = new Ma_ClientSomeoneJoinedChatInformDialogManager(ref ma_client_someone_jioned_chat_inform_dialog_manager_users_list
                , new AuthSend(AllMa_Authsend), new IsLoggedIn(AllMa_IsLoggedIn));

            ma_formal_message_request_dialog_manager = new Ma_FormalMessageRequestDialogManager(ref ma_formal_message_request_dialog_manager_users_list, new AuthSend(AllMa_Authsend)
            , new IsLoggedIn(AllMa_IsLoggedIn), new CreateFormalMessageRequest(AllMa_SendFormalMessageToUser), new IstherUser(AllMa_IstherUser)
            , new CreateOfflineMessage(AllMa_CreateOfflineMessage));

            ma_client_signup_request_dialog_manager = new Ma_ClientSignupRequestDialogManager(ref ma_client_signup_request_dialog_manager_users_list
                , new UnAuthSend(AllMa_UnAuthSend), new IsThereUnauthWorkerThread(AllMa_IsThereUnauthWorkerThread), new SignUp(AllMa_Signup));

            ma_create_add_agreement_dialog_manager = new Ma_CreateAddAgreementDialogManager(ref ma_create_add_agreement_dialog_manager_users_list
                , new AuthSend(AllMa_Authsend), new IsLoggedIn(AllMa_IsLoggedIn), new CreateAddAgreement(AllMa_CreateAddAgreement)
                , new CreateOfflineMessage(AllMa_CreateOfflineMessage));

            ma_get_agreement_answer_dialog_manager = new Ma_GetAgreementAnswerDialogManager(ref ma_get_agreement_answer_dialog_manager_users_list
                , new AuthSend(AllMa_Authsend), new IsLoggedIn(AllMa_IsLoggedIn), new GetAgreementAnswer(AllMa_GetAgreementAnswer));

            ma_client_friend_changed_status_inform_dialog_manager = new Ma_ClientFriendChangedStatusInformDialogManager(ref ma_client_friend_changed_status_inform_dialog_manager_users_list
                , new AuthSend(AllMa_Authsend), new IsLoggedIn(AllMa_IsLoggedIn));

            ma_send_to_client_formal_message_dialog_manager = new Ma_SendToClinetFormalMessageDialogManager(ref ma_send_to_client_formal_message_dialog_manager_users_list
                , new AuthSend(AllMa_Authsend), new IsLoggedIn(AllMa_IsLoggedIn));

            ma_server_read_offline_messages_inform_dialog_manager = new Ma_ServerReadOfflineMessagesInformDialogManager(ref ma_server_read_offline_messages_inform_dialog_manager_users_list
                , new AuthSend(AllMa_Authsend), new IsLoggedIn(AllMa_IsLoggedIn), new OfflineMessagesReadInform(AllMa_OfflineMessagesReadInform));

            ma_client_friend_list_changed_inform_dialog_manager = new Ma_ClientFriendListChangedInformdialogManager(ref ma_client_friend_list_changed_inform_dialog_manager_users_list
                , new AuthSend(AllMa_Authsend), new IsLoggedIn(AllMa_IsLoggedIn));

            ma_client_created_private_chat_inform_dialog_manager = new Ma_ClientCreatedPrivateChatInformDialogManager(ref ma_client_created_private_chat_inform_dialog_manager_users_list, new AuthSend(AllMa_Authsend)
            , new IsLoggedIn(AllMa_IsLoggedIn), new GetPrivateChatInvitationAnswer(AllMa_GetPrivateChatInvitationAnswer), new IsTherePrivateChat(AllMa_IsTherePrivateChat));

            ma_client_invited_agreement_inform_dialog_manager = new Ma_ClientInvitedAgreementInformDialogManager(ref ma_client_invited_agreement_inform_dialog_manager_users_list
                , new AuthSend(AllMa_Authsend), new IsLoggedIn(AllMa_IsLoggedIn), new GetAUserAgreementInvitation(AllMa_GetAUserAgreementInvitation));

        }


        public void ReceiveMessage(BaseServerDialogMessage p_message)
        {
            if (p_message is UnAuthServerDialogMessage)
            {
                UnAuthServerDialogMessage temp_unauth_server_dialog_message = (UnAuthServerDialogMessage)p_message;

                if (temp_unauth_server_dialog_message.Get_message.Get_dialog_type == TypeOfDialog.ClientSignupRequest)
                {
                    ma_client_signup_request_dialog_manager.ReceiveMessage(p_message);
                }
                else if (temp_unauth_server_dialog_message.Get_message.Get_dialog_type == TypeOfDialog.LoginRequest)
                {
                    ma_login_request_dialog_manager.ReceiveMessage(p_message);
                }
            }
            else if (p_message is AuthServerDialogMessage)
            {
                AuthServerDialogMessage temp_auth_server_dialog_message = (AuthServerDialogMessage)p_message;

                if (temp_auth_server_dialog_message.Get_message.Get_dialog_type == TypeOfDialog.ClientFriendChangedStatusInform)
                {
                    ma_client_friend_changed_status_inform_dialog_manager.ReceiveMessage(p_message);
                }
                else if (temp_auth_server_dialog_message.Get_message.Get_dialog_type == TypeOfDialog.ServerReadOfflineMessagesInform)
                {
                    ma_server_read_offline_messages_inform_dialog_manager.ReceiveMessage(p_message);
                }
                else if (temp_auth_server_dialog_message.Get_message.Get_dialog_type == TypeOfDialog.SendToClinetFormalMessage)
                {
                    ma_send_to_client_formal_message_dialog_manager.ReceiveMessage(p_message);
                }
                else if (temp_auth_server_dialog_message.Get_message.Get_dialog_type == TypeOfDialog.ClientJoinPublicChatRequest)
                {
                    ma_client_join_public_chat_request_dialog_manager.ReceiveMessage(p_message);
                }
                else if (temp_auth_server_dialog_message.Get_message.Get_dialog_type == TypeOfDialog.ClientLeaveChatRequest)
                {
                    ma_client_leave_chat_request_dialog_manager.ReceiveMessage(p_message);
                }
                else if (temp_auth_server_dialog_message.Get_message.Get_dialog_type == TypeOfDialog.ClientSomeoneJoinedChatInform)
                {
                    ma_client_someone_jioned_chat_inform_dialog_manager.ReceiveMessage(p_message);
                }
                else if (temp_auth_server_dialog_message.Get_message.Get_dialog_type == TypeOfDialog.ClientSomeoneLeftChatInform)
                {
                    ma_client_someone_left_chat_inform_dialog_manager.ReceiveMessage(p_message);
                }
                else if (temp_auth_server_dialog_message.Get_message.Get_dialog_type == TypeOfDialog.CreateAddAgreement)
                {
                    ma_create_add_agreement_dialog_manager.ReceiveMessage(p_message);
                }
                else if (temp_auth_server_dialog_message.Get_message.Get_dialog_type == TypeOfDialog.CreatePrivateChatRequest)
                {
                    ma_create_private_chat_request_dialog_manager.ReceiveMessage(p_message);
                }
                else if (temp_auth_server_dialog_message.Get_message.Get_dialog_type == TypeOfDialog.FormalMessageRequest)
                {
                    ma_formal_message_request_dialog_manager.ReceiveMessage(p_message);
                }
                else if (temp_auth_server_dialog_message.Get_message.Get_dialog_type == TypeOfDialog.GetAgreementAnswer)
                {
                    ma_get_agreement_answer_dialog_manager.ReceiveMessage(p_message);
                }
                else if (temp_auth_server_dialog_message.Get_message.Get_dialog_type == TypeOfDialog.InformEjectedChatUser)
                {
                    ma_inform_ejected_chat_user_dialog_manager.ReceiveMessage(p_message);
                }
                else if (temp_auth_server_dialog_message.Get_message.Get_dialog_type == TypeOfDialog.LoginDataRequest)
                {
                    ma_login_request_data_dialog_manager.ReceiveMessage(p_message);
                }
                else if (temp_auth_server_dialog_message.Get_message.Get_dialog_type == TypeOfDialog.ClientFriendListChangedInform)
                {
                    ma_client_friend_list_changed_inform_dialog_manager.ReceiveMessage(p_message);
                }
                else if (temp_auth_server_dialog_message.Get_message.Get_dialog_type == TypeOfDialog.ClientCreatedPrivateChatInform)
                {
                    ma_client_created_private_chat_inform_dialog_manager.ReceiveMessage(p_message);
                }
                else if (temp_auth_server_dialog_message.Get_message.Get_dialog_type == TypeOfDialog.ClientInvitedAgreementInform)
                {
                    ma_client_invited_agreement_inform_dialog_manager.ReceiveMessage(p_message);
                }
            }
        }


        public void CreateClientSomeoneLeftChatInform(string p_user_to_inform_name, string p_user_left_chat_name, int p_chat_id, TypeOfChat p_chat_type)
        {
            ma_client_someone_left_chat_inform_dialog_manager.Create(p_user_to_inform_name, p_user_left_chat_name, p_chat_id, p_chat_type);
        }

        public void CreateInformEjectedChatUser(string p_user_name, int p_chat_id_user_ejected_from, string p_ejecting_comment, TypeOfChat p_chat_type)
        {
            ma_inform_ejected_chat_user_dialog_manager.Create(p_user_name, p_chat_id_user_ejected_from, p_ejecting_comment, p_chat_type);
        }

        public void CreateClientSomeoneJoinedChatInform(string p_user_to_inform_name, string p_joined_user_name, int p_chat_id, TypeOfChat p_chat_type)
        {
            ma_client_someone_jioned_chat_inform_dialog_manager.Create(p_user_to_inform_name, p_joined_user_name, p_chat_id, p_chat_type);
        }

        public void CreateClientFriendChangedStatusInform(string p_user_to_inform_name, string p_user_changed_status_name, UserStatus p_new_status)
        {
            ma_client_friend_changed_status_inform_dialog_manager.Create(p_user_to_inform_name, p_user_changed_status_name, p_new_status);
        }

        public void CreateSendToClinetFormalMessage(string p_receiver_user_name, string p_sender_user_name, string p_message_text, int p_message_id)
        {
            ma_send_to_client_formal_message_dialog_manager.Create(p_receiver_user_name, p_sender_user_name, p_message_text, p_message_id);
        }

        public void CreatClientFriendListChangedInform(string p_user_name, List<PersonStatus> p_new_friend_list_and_status)
        {
            ma_client_friend_list_changed_inform_dialog_manager.Create(p_user_name, p_new_friend_list_and_status);
        }

        public void CreateClientCreatedPrivateChatInform(string p_user_name, string p_starter_user_name, int p_chat_id)
        {
            ma_client_created_private_chat_inform_dialog_manager.Create(p_user_name, p_starter_user_name, p_chat_id);
        }

        public void CreateClientInvitedAgreementInform(string p_user_name, int p_agreement_id)
        {
            ma_client_invited_agreement_inform_dialog_manager.Create(p_user_name, p_agreement_id);
        }


        public void RemoveAThreadDialogs(int p_thread_id)
        {
            ma_client_signup_request_dialog_manager.RemoveUserDialog(p_thread_id);
            ma_login_request_dialog_manager.RemoveUserDialog(p_thread_id);
        }

        public void RemoveAUserDialogs(string p_user_name)
        {
            ma_login_request_data_dialog_manager.RemoveUserDialog(p_user_name);
            ma_client_leave_chat_request_dialog_manager.RemoveUserDialog(p_user_name);
            ma_client_someone_left_chat_inform_dialog_manager.RemoveUserDialog(p_user_name);
            ma_inform_ejected_chat_user_dialog_manager.RemoveUserDialog(p_user_name);
            ma_create_private_chat_request_dialog_manager.RemoveUserDialog(p_user_name);
            ma_client_join_public_chat_request_dialog_manager.RemoveUserDialog(p_user_name);
            ma_client_someone_jioned_chat_inform_dialog_manager.RemoveUserDialog(p_user_name);
            ma_formal_message_request_dialog_manager.RemoveUserDialog(p_user_name);
            ma_create_add_agreement_dialog_manager.RemoveUserDialog(p_user_name);
            ma_get_agreement_answer_dialog_manager.RemoveUserDialog(p_user_name);
            ma_client_friend_changed_status_inform_dialog_manager.RemoveUserDialog(p_user_name);
            ma_send_to_client_formal_message_dialog_manager.RemoveUserDialog(p_user_name);
            ma_server_read_offline_messages_inform_dialog_manager.RemoveUserDialog(p_user_name);
            ma_client_friend_list_changed_inform_dialog_manager.RemoveUserDialog(p_user_name);
            ma_client_created_private_chat_inform_dialog_manager.RemoveUserDialog(p_user_name);
            ma_client_invited_agreement_inform_dialog_manager.RemoveUserDialog(p_user_name);
        }


        protected bool AllMa_IsLoggedIn(string p_user_name)
        {
            return is_logged_in(p_user_name);
        }
        protected void AllMa_Authsend(string p_user_name, DialogMessageForClient p_message)
        {
            auth_send_to_user(p_user_name, p_message);
        }
        protected void AllMa_UnAuthSend(int p_thread_id, DialogMessageForClient p_message)
        {
            unauth_send_to_usrer(p_thread_id, p_message);
        }
        protected bool AllMa_IsThereUnauthWorkerThread(int p_thread_id)
        {
            return is_there_unauththread(p_thread_id);
        }
        public List<AgreementInvitationInfo> AllMa_GetAllUserAgreementInvitation(string p_user_name)
        {
            return get_all_user_agreement_invitation(p_user_name);
        }
        public List<OfflineMessage> AllMa_GetAllUserOfflineMessages(string p_user_name)
        {
            return get_all_user_offline_message(p_user_name);
        }
        public List<int> AllMa_GetPublicChatIds()
        {
            return get_public_chat_ids();
        }
        public UserStatus AllMa_GetUserStatus(string p_user_name)
        {
            return get_user_status(p_user_name);
        }
        public List<string> AllMa_GetUserfriendList(string p_user_name)
        {
            return get_user_friends_list(p_user_name);
        }
        public Se_BaseBooleanFunctionResult AllMa_Login(int p_thread_id, string p_user_name, string p_password)
        {
            return server_login(p_thread_id, p_user_name, p_password);
        }
        public void AllMa_ServerInformChatLeave(string p_user_name, int p_chat_id, TypeOfChat p_chat_type)
        {
            server_inform_chat_leave(p_user_name, p_chat_id, p_chat_type);
        }
        public Se_BaseBooleanFunctionResult AllMa_AreFriends(string p_first_person, string p_second_person)
        {
            return are_friends(p_first_person, p_second_person);
        }
        public Se_BaseIntFunctionResult AllMa_CreatePrivateChat(string p_first_person, string p_second_person)
        {
            return create_private_chat(p_first_person, p_second_person);
        }
        public bool AllMa_IsTherePrivateChat(int p_chat_id)
        {
            return is_there_private_chat(p_chat_id);
        }
        public void AllMa_startClientCreatedPrivateChatInform(string p_user_name, string p_starter_user_name, int p_chat_id)
        {
            start_client_created_private_chat_inform(p_user_name, p_starter_user_name, p_chat_id);
        }
        public Se_BaseBooleanFunctionResult AllMa_JoinPublicChatRequest(string p_user_name, int p_chat_id)
        {
            return join_public_chat_request(p_user_name, p_chat_id);
        }
        public bool AllMa_IsUserInPublicChat(string p_user_name, int p_chat_id)
        {
            return is_user_in_public_chat(p_user_name, p_chat_id);
        }
        public List<string> AllMa_GetPublicChatUsersList(int p_chat_id)
        {
            return get_public_chat_users_list(p_chat_id);
        }
        public void AllMa_SendFormalMessageToUser(FormalMessage p_message, string p_receiver_user_name)
        {
            send_fromal_message_to_user(p_message, p_receiver_user_name);
        }
        public bool AllMa_IstherUser(string p_user_name)
        {
            return user_exist(p_user_name);
        }
        public void AllMa_CreateOfflineMessage(string p_receiver_user_name, OfflineMessage p_message)
        {
            create_offline_message(p_receiver_user_name, p_message);
        }
        public Se_BaseBooleanFunctionResult AllMa_Signup(string p_user_name, string p_password)
        {
            return signup_request(p_user_name, p_password);
        }
        public Se_BaseBooleanFunctionResult AllMa_CreateAddAgreement(string p_starter_user_name, string p_invited_user_name)
        {
            return create_add_agreement(p_starter_user_name, p_invited_user_name);
        }
        public void AllMa_GetAgreementAnswer(string p_user_name, int p_agreement_id, bool p_answer, TypeOfAgreement p_agreement_type)
        {
            get_agreement_answer(p_user_name, p_agreement_id, p_answer, p_agreement_type);
        }
        public void AllMa_OfflineMessagesReadInform(string p_user_name, List<int> p_message_ids)
        {
            offline_message_read_inform(p_user_name, p_message_ids);
        }
        public Se_BaseBooleanFunctionResult AllMa_GetPrivateChatInvitationAnswer(string p_user_name, int p_chat_id, bool p_answer)
        {
            return get_private_chat_invitation_answer(p_user_name, p_chat_id, p_answer);
        }
        public AgreementInvitationInfo AllMa_GetAUserAgreementInvitation(string p_user_name, int p_agreement_id)
        {
            return get_a_user_agreement_invitation(p_user_name, p_agreement_id);
        }

    }

    public class Se_ServerDelegateForDialogs
    {
        public GetFriendsList get_user_friends_list;
        public GetUserStatus get_user_status;
        public GetPublicChatIds get_public_chat_ids;
        public GetOfflineMessages get_all_user_offline_message;
        public GetAllAgreementInvitation get_all_user_agreement_invitation;
        public AuthSend auth_send_to_user;
        public IsLoggedIn is_logged_in;
        public UnAuthSend unauth_send_to_usrer;
        public IsThereUnauthWorkerThread is_there_unauththread;
        public Login server_login;
        public ServerInformChatLeave server_inform_chat_leave;
        public OnlineAreFriends are_friends;
        public CreatePrivateChat create_private_chat;
        public IsTherePrivateChat is_there_private_chat;
        public StartClientCreatedPrivateChatInform start_client_created_private_chat_inform;
        public JoinPublicChatRequest join_public_chat_request;
        public IsUserInPublicChat is_user_in_public_chat;
        public GetPublicChatUsersList get_public_chat_users_list;
        public CreateFormalMessageRequest send_fromal_message_to_user;
        public IstherUser user_exist;
        public CreateOfflineMessage create_offline_message;
        public SignUp signup_request;
        public CreateAddAgreement create_add_agreement;
        public GetAgreementAnswer get_agreement_answer;
        public OfflineMessagesReadInform offline_message_read_inform;
        public GetPrivateChatInvitationAnswer get_private_chat_invitation_answer;
        public GetAUserAgreementInvitation get_a_user_agreement_invitation;

        public Se_ServerDelegateForDialogs(GetFriendsList p_get_user_friends_list, GetUserStatus p_get_user_status, GetPublicChatIds p_get_public_chat_ids, GetOfflineMessages p_get_all_user_offline_message
        , GetAllAgreementInvitation p_get_all_user_agreement_invitation, AuthSend p_auth_send_to_user, IsLoggedIn p_is_logged_in, UnAuthSend p_unauth_send_to_usrer, IsThereUnauthWorkerThread p_is_there_unauththread
        , Login p_server_login, ServerInformChatLeave p_server_inform_chat_leave, OnlineAreFriends p_are_friends, CreatePrivateChat p_create_private_chat, IsTherePrivateChat p_is_there_private_chat
        , StartClientCreatedPrivateChatInform p_start_client_created_private_chat_inform, JoinPublicChatRequest p_join_public_chat_request, IsUserInPublicChat p_is_user_in_public_chat, GetPublicChatUsersList p_get_public_chat_users_list
        , CreateFormalMessageRequest p_send_fromal_message_to_user, IstherUser p_user_exist, CreateOfflineMessage p_create_offline_message, SignUp p_signup_request, CreateAddAgreement p_create_add_agreement
        , GetAgreementAnswer p_get_agreement_answer, OfflineMessagesReadInform p_offline_message_read_inform, GetPrivateChatInvitationAnswer p_get_private_chat_invitation_answer
            , GetAUserAgreementInvitation p_get_a_user_agreement_invitation)
        {
            get_user_friends_list = p_get_user_friends_list;
            get_user_status = p_get_user_status;
            get_public_chat_ids = p_get_public_chat_ids;
            get_all_user_offline_message = p_get_all_user_offline_message;
            get_all_user_agreement_invitation = p_get_all_user_agreement_invitation;
            auth_send_to_user = p_auth_send_to_user;
            is_logged_in = p_is_logged_in;
            unauth_send_to_usrer = p_unauth_send_to_usrer;
            is_there_unauththread = p_is_there_unauththread;
            server_login = p_server_login;
            server_inform_chat_leave = p_server_inform_chat_leave;
            are_friends = p_are_friends;
            create_private_chat = p_create_private_chat;
            is_there_private_chat = p_is_there_private_chat;
            start_client_created_private_chat_inform = p_start_client_created_private_chat_inform;
            join_public_chat_request = p_join_public_chat_request;
            is_user_in_public_chat = p_is_user_in_public_chat;
            get_public_chat_users_list = p_get_public_chat_users_list;
            send_fromal_message_to_user = p_send_fromal_message_to_user;
            user_exist = p_user_exist;
            create_offline_message = p_create_offline_message;
            signup_request = p_signup_request;
            create_add_agreement = p_create_add_agreement;
            get_agreement_answer = p_get_agreement_answer;
            offline_message_read_inform = p_offline_message_read_inform;
            get_private_chat_invitation_answer = p_get_private_chat_invitation_answer;
            get_a_user_agreement_invitation = p_get_a_user_agreement_invitation;
        }
    }


    public enum DialogStatus
    {
        Running,
        WaitingForAMessage,
        WaitingForAReceipt,
        ReceiptInvestigation,
        ReceipptRejected,
        ReceiptAccepted,
        MessageInvestigation,
        MessageAccepted,
        MessageRejected,
        End,
        Canceling
    }

    public abstract class Se_BaseBooleanFunctionResult
    {
        protected bool function_result;

    }
    public class Se_BooleanFunctionAccResult : Se_BaseBooleanFunctionResult
    {
        public Se_BooleanFunctionAccResult()
        {
            function_result = true;
        }
        public bool get_function_result
        {
            get
            {
                return function_result;
            }
        }
    }
    public class Se_BooleanFunctionRejResult : Se_BaseBooleanFunctionResult
    {
        string reject_comment;
        public Se_BooleanFunctionRejResult(string p_reject_comment)
        {
            function_result = false;
            reject_comment = p_reject_comment;
        }
        public string get_reject_comment
        {
            get
            {
                return reject_comment;
            }
        }
        public bool get_function_result
        {
            get
            {
                return function_result;
            }
        }
    }

    public abstract class Se_BaseIntFunctionResult
    {
        protected bool function_result;

    }
    public class Se_IntFunctionRejResult : Se_BaseIntFunctionResult
    {
        string reject_comment;
        public string Get_reject_comment
        {
            get { return reject_comment; }
        }

        public Se_IntFunctionRejResult(string p_reject_comment)
        {
            reject_comment = p_reject_comment;
            function_result = true;
        }
        public bool get_function_result
        {
            get
            {
                return function_result;
            }
        }
    }
    public class Se_IntFunctionAccResult : Se_BaseIntFunctionResult
    {
        int message_content;
        public Se_IntFunctionAccResult(int p_message_content)
        {
            function_result = false;
            message_content = p_message_content;
        }
        public int get_message_content
        {
            get
            {
                return message_content;
            }
        }
        public bool get_function_result
        {
            get
            {
                return function_result;
            }
        }
    }

    public static class HelperFunctions
    {
        public static int GetGUID()
        {
            Guid t_guid = new Guid();
            t_guid = Guid.NewGuid();
            return BitConverter.ToInt32(t_guid.ToByteArray(), 0);
        }

        public static bool DialogMessageObjectInvestigate(DialogMessageForServer p_dialog_message)
        {
            try
            {
                if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.ReceiptMessage)
                {
                    Di_Mess_ReceiptMessage temp = (Di_Mess_ReceiptMessage)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.AgreementAnswer)
                {
                    Di_Mess_AgreementAnswer temp = (Di_Mess_AgreementAnswer)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.InformEjectedChatUser)
                {
                    Di_Mess_InformEjectedChatUser temp = (Di_Mess_InformEjectedChatUser)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.CreateAddAgreementRequest)
                {
                    Di_Mess_CreateAddAgreementRequest temp = (Di_Mess_CreateAddAgreementRequest)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.CreatePrivateChatCommand)
                {
                    Di_Mess_CreatePrivateChatCommand temp = (Di_Mess_CreatePrivateChatCommand)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.FriendChangeStatus)
                {
                    Di_Mess_FriendChangeStatus temp = (Di_Mess_FriendChangeStatus)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.FriendsListAndStatus)
                {
                    Di_Mess_FriendsListAndStatus temp = (Di_Mess_FriendsListAndStatus)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.InviteToAgreementInfo)
                {
                    Di_Mess_InviteToAgreemenstInfo temp = (Di_Mess_InviteToAgreemenstInfo)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.JoinPublicChatRequest)
                {
                    Di_Mess_JoinPublicChatRequest temp = (Di_Mess_JoinPublicChatRequest)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.LoginRequestData)
                {
                    Di_Mess_LoginRequestData temp = (Di_Mess_LoginRequestData)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.OfflineMessages)
                {
                    Di_Mess_OfflineMessages temp = (Di_Mess_OfflineMessages)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.PrivateChatInfo)
                {
                    Di_Mess_PrivateChatInfo temp = (Di_Mess_PrivateChatInfo)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.PublicChatIds)
                {
                    Di_Mess_PublicChatIds temp = (Di_Mess_PublicChatIds)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.SignUpRequestData)
                {
                    Di_Mess_SignUpRequestData temp = (Di_Mess_SignUpRequestData)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.StartPrivateChatRequest)
                {
                    Di_Mess_StartPrivateChatRequest temp = (Di_Mess_StartPrivateChatRequest)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.LoginDataRequestMessage)
                {
                    Di_Mess_LoginDataRequestMessage temp = (Di_Mess_LoginDataRequestMessage)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.CancelDialog)
                {
                    Di_Mess_CancelDialogMessage temp = (Di_Mess_CancelDialogMessage)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.ClientLeaveChatRequest)
                {
                    Di_Mess_ClientLeaveChatRequest temp = (Di_Mess_ClientLeaveChatRequest)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.SomeoneLeftTheChat)
                {
                    Di_Mess_SomeoneLeftTheChat temp = (Di_Mess_SomeoneLeftTheChat)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.ClientInformFormalMessage)
                {
                    Di_Mess_ClientInformFormalMessage temp = (Di_Mess_ClientInformFormalMessage)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.ClientFormalMessageRequest)
                {
                    Di_Mess_ClientFormalMessageRequest temp = (Di_Mess_ClientFormalMessageRequest)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.SomeoneJoinedTheChat)
                {
                    Di_Mess_SomeoneJoinedTheChat temp = (Di_Mess_SomeoneJoinedTheChat)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.PublicChatUsersIds)
                {
                    Di_Mess_PublicChatUsersIds temp = (Di_Mess_PublicChatUsersIds)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.PrivateChatInvitationAnswer)
                {
                    Di_Mess_PrivateChatInvitationAnswer temp = (Di_Mess_PrivateChatInvitationAnswer)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.CreatedPrivateChatInform)
                {
                    Di_Mess_CreatedPrivateChatInform temp = (Di_Mess_CreatedPrivateChatInform)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.InformInviteToAgreementInfo)
                {
                    Di_Mess_InformInviteToAgreementInfo temp = (Di_Mess_InformInviteToAgreementInfo)p_dialog_message.Get_message_object;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool DialogMessageObjectInvestigate(DialogMessageForClient p_dialog_message)
        {
            try
            {

                if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.ReceiptMessage)
                {
                    Di_Mess_ReceiptMessage temp = (Di_Mess_ReceiptMessage)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.AgreementAnswer)
                {
                    Di_Mess_AgreementAnswer temp = (Di_Mess_AgreementAnswer)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.InformEjectedChatUser)
                {
                    Di_Mess_InformEjectedChatUser temp = (Di_Mess_InformEjectedChatUser)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.CreateAddAgreementRequest)
                {
                    Di_Mess_CreateAddAgreementRequest temp = (Di_Mess_CreateAddAgreementRequest)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.CreatePrivateChatCommand)
                {
                    Di_Mess_CreatePrivateChatCommand temp = (Di_Mess_CreatePrivateChatCommand)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.FriendChangeStatus)
                {
                    Di_Mess_FriendChangeStatus temp = (Di_Mess_FriendChangeStatus)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.FriendsListAndStatus)
                {
                    Di_Mess_FriendsListAndStatus temp = (Di_Mess_FriendsListAndStatus)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.InviteToAgreementInfo)
                {
                    Di_Mess_InviteToAgreemenstInfo temp = (Di_Mess_InviteToAgreemenstInfo)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.JoinPublicChatRequest)
                {
                    Di_Mess_JoinPublicChatRequest temp = (Di_Mess_JoinPublicChatRequest)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.LoginRequestData)
                {
                    Di_Mess_LoginRequestData temp = (Di_Mess_LoginRequestData)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.OfflineMessages)
                {
                    Di_Mess_OfflineMessages temp = (Di_Mess_OfflineMessages)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.PrivateChatInfo)
                {
                    Di_Mess_PrivateChatInfo temp = (Di_Mess_PrivateChatInfo)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.PublicChatIds)
                {
                    Di_Mess_PublicChatIds temp = (Di_Mess_PublicChatIds)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.SignUpRequestData)
                {
                    Di_Mess_SignUpRequestData temp = (Di_Mess_SignUpRequestData)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.StartPrivateChatRequest)
                {
                    Di_Mess_StartPrivateChatRequest temp = (Di_Mess_StartPrivateChatRequest)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.LoginDataRequestMessage)
                {
                    Di_Mess_LoginDataRequestMessage temp = (Di_Mess_LoginDataRequestMessage)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.CancelDialog)
                {
                    Di_Mess_CancelDialogMessage temp = (Di_Mess_CancelDialogMessage)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.ClientLeaveChatRequest)
                {
                    Di_Mess_ClientLeaveChatRequest temp = (Di_Mess_ClientLeaveChatRequest)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.SomeoneLeftTheChat)
                {
                    Di_Mess_SomeoneLeftTheChat temp = (Di_Mess_SomeoneLeftTheChat)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.ClientInformFormalMessage)
                {
                    Di_Mess_ClientInformFormalMessage temp = (Di_Mess_ClientInformFormalMessage)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.ClientFormalMessageRequest)
                {
                    Di_Mess_ClientFormalMessageRequest temp = (Di_Mess_ClientFormalMessageRequest)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.SomeoneJoinedTheChat)
                {
                    Di_Mess_SomeoneJoinedTheChat temp = (Di_Mess_SomeoneJoinedTheChat)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.PublicChatUsersIds)
                {
                    Di_Mess_PublicChatUsersIds temp = (Di_Mess_PublicChatUsersIds)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.PrivateChatInvitationAnswer)
                {
                    Di_Mess_PrivateChatInvitationAnswer temp = (Di_Mess_PrivateChatInvitationAnswer)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.CreatedPrivateChatInform)
                {
                    Di_Mess_CreatedPrivateChatInform temp = (Di_Mess_CreatedPrivateChatInform)p_dialog_message.Get_message_object;
                }
                else if (p_dialog_message.Get_message_object_type == TypeOfDialogMessage.InformInviteToAgreementInfo)
                {
                    Di_Mess_InformInviteToAgreementInfo temp = (Di_Mess_InformInviteToAgreementInfo)p_dialog_message.Get_message_object;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool CheckBaseServerDialogMessage(BaseServerDialogMessage p_message)
        {

            try
            {
                if (p_message is AuthServerDialogMessage)
                {
                    AuthServerDialogMessage temp_auth_server = (AuthServerDialogMessage)p_message;
                }
                else if (p_message is UnAuthServerDialogMessage)
                {
                    UnAuthServerDialogMessage temp_auth_server = (UnAuthServerDialogMessage)p_message;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }


}










