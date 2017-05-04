using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerTcpChat.Classes;
using System.Collections.Generic;
using System.Threading;
using CommonChatTypes;


namespace ServerTcpChatUnitTest
{
    [TestClass]
    public class AllDialogsUnitTest
    {
        List<DialogMessageForClient> messages_sent_from_dialog;

        DialogMessageForServer last_message_sent_to_dialog;

        BaseDialog test_dialog;

        string test_user_name; 
        int test_thread_id;

        int last_message_sent_to_dialg_id;
        int last_message_received_from_dialog_id;
        int last_message_number;

        int test_dialog_id;
        //int test_retry_counts;
        int test_max_retry_counts;
        TypeOfDialog test_dialog_type;
        int test_level_counts;

        int auth_send_calls_counts;
        int unauth_send_calls_count;
        int get_friends_list_calls_counts;
        int get_user_status_calls_counts;
        int get_public_chat_ids_calls_count;
        int get_offline_messages_calls_counts;
        int get_agreement_invitation_calls_counts;
        int remove_calls_counts;
        int is_logged_in_calls_counts;
        int login_calls_count;
        int is_there_unauth_worker_thread_calls_count;
        int create_add_agreement_calls_count;
        int create_offline_message_calls_count;

        int get_private_chat_invitation_answer_calls_count;
        int offline_messages_read_inform_calls_count;
        int get_agreement_answer_calls_count;
        int signup_calls_count;
        int is_there_user_calls_count;
        int create_formal_message_request_calls_count;
        int get_public_chat_users_list_calls_count;
        int is_user_in_public_chat_calls_count;
        int join_public_chat_request_calls_count;
        int start_client_created_private_chat_inform_calls_count;
        int is_there_private_chat_calls_count;
        int create_private_chat_calls_count;
        int online_are_friends_calls_count;
        int server_inform_chat_leave_calls_count;

        //string send_destination;
        //int remove calls number

        public AllDialogsUnitTest()
        {
            messages_sent_from_dialog = new List<DialogMessageForClient>();

            last_message_sent_to_dialog = null;

            last_message_number = 0;
            last_message_received_from_dialog_id = 0;
            last_message_sent_to_dialg_id = 0;

            auth_send_calls_counts = 0;
            get_friends_list_calls_counts = 0;
            get_user_status_calls_counts = 0;
            get_public_chat_ids_calls_count = 0;
            get_offline_messages_calls_counts = 0;
            get_agreement_invitation_calls_counts = 0;
            remove_calls_counts = 0;
            is_logged_in_calls_counts = 0;
            login_calls_count = 0;
            is_there_unauth_worker_thread_calls_count = 0;
            unauth_send_calls_count = 0;
            create_add_agreement_calls_count = 0;
            create_offline_message_calls_count = 0;
            get_private_chat_invitation_answer_calls_count = 0;
            offline_messages_read_inform_calls_count = 0;
            get_agreement_answer_calls_count = 0;
            signup_calls_count = 0;
            is_there_user_calls_count = 0;
            create_formal_message_request_calls_count = 0;
            get_public_chat_users_list_calls_count = 0;
            is_user_in_public_chat_calls_count = 0;
            join_public_chat_request_calls_count = 0;
            start_client_created_private_chat_inform_calls_count = 0;
            is_there_private_chat_calls_count = 0;
            create_private_chat_calls_count = 0;
            online_are_friends_calls_count = 0;
            server_inform_chat_leave_calls_count = 0;
        }

        [TestMethod]
        public void AllDialogs_Test()
        {
            //Se_ServerDelegateForDialogs delegates_for_dialogs = new Se_ServerDelegateForDialogs(
            Se_ServerDelegateForDialogs server_delegates_for_dialogs = new Se_ServerDelegateForDialogs(new GetFriendsList(GetFriendsList), new GetUserStatus(GetUserStatus)
            , new GetPublicChatIds(GetPublicChatsIds), new GetOfflineMessages(GetOfflineMessages), new GetAllAgreementInvitation(GetAgreementInvitation)
            , new AuthSend(AuthSend), new IsLoggedIn(IsLoggedIn), new UnAuthSend(UnAuthSend), new IsThereUnauthWorkerThread(IsThereUnauthWorkerThread)
            , new Login(Login), new ServerInformChatLeave(ServerInformChatLeave), new OnlineAreFriends(OnlineAreFriends), new CreatePrivateChat(CreatePrivateChat)
            , new IsTherePrivateChat(IsTherePrivateChat), new StartClientCreatedPrivateChatInform(StartClientCreatedPrivateChatInform)
            , new JoinPublicChatRequest(JoinPublicChatRequest), new IsUserInPublicChat(IsUserInPublicChat), new GetPublicChatUsersList(GetPublicChatUsersList)
            , new CreateFormalMessageRequest(CreateFormalMessageRequest), new IstherUser(IstherUser), new CreateOfflineMessage(CreateOfflineMessage)
            , new SignUp(SignUp), new CreateAddAgreement(CreateAddAgreement), new GetAgreementAnswer(GetAgreementAnswer), new OfflineMessagesReadInform(OfflineMessagesReadInform)
            , new GetPrivateChatInvitationAnswer(GetPrivateChatInvitationAnswer),new GetAUserAgreementInvitation(Test_GetAUserAgreementtInvitation));

            Dictionary<TypeOfDialog, Dictionary<int, Se_AuthDialog>> all_auth_dialogs = CreateAllAuthDialogs();
            Dictionary<TypeOfDialog, Dictionary<int, Se_UnAuthDialog>> all_unauth_dialogs = CreateAllUnAuthDialogs();

            AllDialogs all_dialogs = new AllDialogs(server_delegates_for_dialogs, ref all_auth_dialogs, ref all_unauth_dialogs);

            int first_dialog_id = HelperFunctions.GetGUID();
            string first_dialog_user_name = "ali";
            Di_Server_LoginDataRequest first_login_data_request_dialog = new Di_Server_LoginDataRequest(first_dialog_id, first_dialog_user_name,
                new AuthSend(AuthSend), new GetFriendsList(GetFriendsList), new GetUserStatus(GetUserStatus), new GetPublicChatIds(GetPublicChatsIds)
                , new GetOfflineMessages(GetOfflineMessages), new GetAllAgreementInvitation(GetAgreementInvitation), new Remove(Remove)
                , new IsLoggedIn(IsLoggedIn));

            int second_dialog_id = HelperFunctions.GetGUID();

            int first_message_id = HelperFunctions.GetGUID();
            Di_Mess_LoginDataRequestMessage first_login_data_request_message = new Di_Mess_LoginDataRequestMessage();
            DialogMessageForServer first_dialog_message = new DialogMessageForServer(first_message_id, first_dialog_id, 1, TypeOfDialog.LoginDataRequest
                , first_login_data_request_message, TypeOfDialogMessage.LoginDataRequestMessage);
            AuthServerDialogMessage first_server_dialog_message = new AuthServerDialogMessage(first_dialog_message, first_dialog_user_name);

            int second_message_id = HelperFunctions.GetGUID();
            all_dialogs.ReceiveMessage(first_server_dialog_message);
            DialogMessageForServer second_dialog_message = new DialogMessageForServer(second_message_id, second_dialog_id, 1, TypeOfDialog.LoginDataRequest
               , first_login_data_request_message, TypeOfDialogMessage.LoginRequestData);
            AuthServerDialogMessage second_server_dialog_message = new AuthServerDialogMessage(second_dialog_message, first_dialog_user_name);

            all_dialogs.ReceiveMessage(second_server_dialog_message);

            all_dialogs.CreatClientFriendListChangedInform(first_dialog_user_name, new List<PersonStatus>());

            all_dialogs.RemoveAUserDialogs(first_dialog_user_name);


        }

        /*[TestMethod]
        public void Di_Server_CreateAddAgreement_Test()
        {
            ReaderWriterLockSlim all_agreements_lock = new ReaderWriterLockSlim();
            ReaderWriterLockSlim dbcontroller_lock = new ReaderWriterLockSlim();

            int dialog_id = GetGuid();
            test_dialog_id = dialog_id;

            string user_name = "farhad";
            test_user_name = user_name;

            int max_retry_counts = 3;
            test_max_retry_counts = max_retry_counts;

            TypeOfDialog dialog_type = TypeOfDialog.CreateAddAgreement;
            test_dialog_type = dialog_type;

            int level_counts = 1;
            test_level_counts = level_counts;

            test_dialog = new Di_Server_CreateAddAgreement(test_dialog_id, test_user_name, new AuthSend(AuthSend), new Remove(Remove), new CreateAddAgreement(CreateAddAgreement)
            , new CreateOfflineMessage(CreateOfflineMessage), new IsLoggedIn(IsLoggedIn));

            Assert.AreEqual(DialogLevelType.WaitingForMessageReceive, test_dialog.Get_all_dialog_levels[1].Get_level_type);

            TestCommonAuthDialogVariables();
            TestSpecialVariables(1, 0, null, 0, new List<DialogMessageForServer>(), DialogStatus.WaitingForAMessage, new ExceptionOccurenceStruct());

            //sending starting message
            Di_Mess_CreateAddAgreementRequest create_add_agreement_request_message_object = new Di_Mess_CreateAddAgreementRequest("ahmad");
            SendToDialog(TypeOfDialogMessage.CreateAddAgreementRequest, create_add_agreement_request_message_object);

            TestCommonAuthDialogVariables();
            TestSpecialVariables(1, 0, last_message_sent_to_dialog, last_message_received_from_dialog_id, new List<DialogMessageForServer>(), DialogStatus.End, new ExceptionOccurenceStruct());

            Assert.AreEqual(1, create_add_agreement_calls_count);
            Assert.AreEqual(1, is_logged_in_calls_counts);
            Assert.AreEqual(1, messages_sent_from_dialog.Count);

            TestDialogMessage(messages_sent_from_dialog[0], 2, TypeOfDialogMessage.ReceiptMessage);

        }

        [TestMethod]
        public void Di_Server_LoginRequest_Test()
        {
            int dialog_id = GetGuid();
            test_dialog_id = dialog_id;

            int thread_id = GetGuid();
            test_thread_id = thread_id;

            int max_retry_counts = 6;
            test_max_retry_counts = max_retry_counts;

            TypeOfDialog dialog_type = TypeOfDialog.LoginRequest;
            test_dialog_type = dialog_type;

            int level_counts = 1;
            test_level_counts = level_counts;

            ReaderWriterLockSlim db_controller_lock = new ReaderWriterLockSlim();

            test_dialog = new Di_Server_LoginRequest(dialog_id, thread_id, new Remove(Remove), new Login(Login), new UnAuthSend(UnAuthSend), new IsThereUnauthWorkerThread(IsThereUnauthWorkerThread));

            TestLockToBeeFree(db_controller_lock);

            Assert.AreEqual(DialogLevelType.WaitingForMessageReceive, test_dialog.Get_all_dialog_levels[1].Get_level_type);

            TestCommonUnauAuthDialogVariables();
            TestSpecialVariables(1, 0, null, 0, new List<DialogMessageForServer>(), DialogStatus.WaitingForAMessage, new ExceptionOccurenceStruct());

            //sending atrting message 
            SendToDialog(TypeOfDialogMessage.LoginRequestData, new Di_Mess_LoginRequestData("ali", "123"));

            Assert.AreEqual(1, messages_sent_from_dialog.Count);
            TestDialogMessage(messages_sent_from_dialog[0], last_message_number, TypeOfDialogMessage.ReceiptMessage);

            Assert.AreEqual(1, is_there_unauth_worker_thread_calls_count);
            Assert.AreEqual(1, login_calls_count);
            Assert.AreEqual(1, remove_calls_counts);
            Assert.AreEqual(1, unauth_send_calls_count);

            TestCommonUnauAuthDialogVariables();
            TestSpecialVariables(1, 0, last_message_sent_to_dialog, 0, new List<DialogMessageForServer>(), DialogStatus.End
                , new ExceptionOccurenceStruct());
        }

        [TestMethod]
        public void Di_Server_LoginDataRequest_Test()
        {
            int dialog_id = GetGuid();
            test_dialog_id = dialog_id;

            string user_name = "farhad";
            test_user_name = user_name;

            int max_retry_counts = 3;
            test_max_retry_counts = max_retry_counts;

            TypeOfDialog dialog_type = TypeOfDialog.LoginDataRequest;
            test_dialog_type = dialog_type;

            int level_counts = 5;
            test_level_counts = level_counts;


            test_dialog = new Di_Server_LoginDataRequest(dialog_id, user_name, new AuthSend(AuthSend), new GetFriendsList(GetFriendsList),
                new GetUserStatus(GetUserStatus), new GetPublicChatIds(GetPublicChatsIds), new GetOfflineMessages(GetOfflineMessages), new GetAllAgreementInvitation(GetAgreementInvitation)
                , new Remove(Remove), new IsLoggedIn(IsLoggedIn));

            for (int i = 1; i <= test_dialog.Get_all_dialog_levels.Count; i++)
            {
                if (i == 1)
                {
                    Assert.AreEqual(test_dialog.Get_all_dialog_levels[i].Get_level_type, DialogLevelType.WaitingForMessageReceive);
                }
                else
                {
                    Assert.AreEqual(test_dialog.Get_all_dialog_levels[i].Get_level_type, DialogLevelType.SendingAMessage);
                }
            }

            TestCommonAuthDialogVariables();
            TestSpecialVariables(1, 0, null, 0, new List<DialogMessageForServer>(), DialogStatus.WaitingForAMessage, new ExceptionOccurenceStruct());


            //sending starting mssage to dialog
            Di_Mess_LoginDataRequestMessage login_data_request_message_object = new Di_Mess_LoginDataRequestMessage();
            SendToDialog(TypeOfDialogMessage.LoginDataRequestMessage, login_data_request_message_object);

            Assert.AreEqual(2, auth_send_calls_counts);
            Assert.AreEqual(1, get_friends_list_calls_counts);
            Assert.AreEqual(2, is_logged_in_calls_counts);
            Assert.AreEqual(2, messages_sent_from_dialog.Count);

            TestDialogMessage(messages_sent_from_dialog[0], last_message_number - 1, TypeOfDialogMessage.ReceiptMessage);
            TestDialogMessage(messages_sent_from_dialog[1], last_message_number, TypeOfDialogMessage.FriendsListAndStatus);

            TestCommonAuthDialogVariables();
            TestSpecialVariables(2, 0, last_message_sent_to_dialog, last_message_received_from_dialog_id, new List<DialogMessageForServer>(), DialogStatus.WaitingForAReceipt, new ExceptionOccurenceStruct());

            //sending first receipt for going from level 2 to level 3
            Di_Mess_ReceiptMessage temp_receipt = new Di_Mess_ReceiptMessage(ReceiptStatus.Accepted, new Di_Mess_Rec_AcceptMessage(last_message_received_from_dialog_id));
            SendToDialog(TypeOfDialogMessage.ReceiptMessage, temp_receipt);

            Assert.AreEqual(3, auth_send_calls_counts);
            Assert.AreEqual(1, get_friends_list_calls_counts);
            Assert.AreEqual(3, is_logged_in_calls_counts);
            Assert.AreEqual(1, get_public_chat_ids_calls_count);

            Assert.AreEqual(3, messages_sent_from_dialog.Count);
            TestDialogMessage(messages_sent_from_dialog[2], last_message_number, TypeOfDialogMessage.PublicChatIds);

            TestCommonAuthDialogVariables();
            TestSpecialVariables(3, 0, last_message_sent_to_dialog, last_message_received_from_dialog_id, new List<DialogMessageForServer>(), DialogStatus.WaitingForAReceipt, new ExceptionOccurenceStruct());

            //az 3 be 4
            temp_receipt = new Di_Mess_ReceiptMessage(ReceiptStatus.Accepted, new Di_Mess_Rec_AcceptMessage(last_message_received_from_dialog_id));
            SendToDialog(TypeOfDialogMessage.ReceiptMessage, temp_receipt);

            Assert.AreEqual(4, auth_send_calls_counts);
            Assert.AreEqual(1, get_friends_list_calls_counts);
            Assert.AreEqual(4, is_logged_in_calls_counts);
            Assert.AreEqual(1, get_public_chat_ids_calls_count);
            Assert.AreEqual(1, get_agreement_invitation_calls_counts);

            Assert.AreEqual(4, messages_sent_from_dialog.Count);
            TestDialogMessage(messages_sent_from_dialog[3], last_message_number, TypeOfDialogMessage.InviteToAgreementInfo);

            TestCommonAuthDialogVariables();
            TestSpecialVariables(4, 0, last_message_sent_to_dialog, last_message_received_from_dialog_id, new List<DialogMessageForServer>(), DialogStatus.WaitingForAReceipt, new ExceptionOccurenceStruct());

            //az 4 be 5
            temp_receipt = new Di_Mess_ReceiptMessage(ReceiptStatus.Accepted, new Di_Mess_Rec_AcceptMessage(last_message_received_from_dialog_id));
            SendToDialog(TypeOfDialogMessage.ReceiptMessage, temp_receipt);

            Assert.AreEqual(5, auth_send_calls_counts);
            Assert.AreEqual(1, get_friends_list_calls_counts);
            Assert.AreEqual(5, is_logged_in_calls_counts);
            Assert.AreEqual(1, get_public_chat_ids_calls_count);
            Assert.AreEqual(1, get_agreement_invitation_calls_counts);
            Assert.AreEqual(1, get_offline_messages_calls_counts);

            Assert.AreEqual(5, messages_sent_from_dialog.Count);
            TestDialogMessage(messages_sent_from_dialog[4], last_message_number, TypeOfDialogMessage.OfflineMessages);

            TestCommonAuthDialogVariables();
            TestSpecialVariables(5, 0, last_message_sent_to_dialog, last_message_received_from_dialog_id, new List<DialogMessageForServer>(), DialogStatus.WaitingForAReceipt, new ExceptionOccurenceStruct());

            //5 be End()
            temp_receipt = new Di_Mess_ReceiptMessage(ReceiptStatus.Accepted, new Di_Mess_Rec_AcceptMessage(last_message_received_from_dialog_id));
            SendToDialog(TypeOfDialogMessage.ReceiptMessage, temp_receipt);

            Assert.AreEqual(5, auth_send_calls_counts);
            Assert.AreEqual(1, get_friends_list_calls_counts);
            Assert.AreEqual(5, is_logged_in_calls_counts);
            Assert.AreEqual(1, get_public_chat_ids_calls_count);
            Assert.AreEqual(1, get_agreement_invitation_calls_counts);
            Assert.AreEqual(1, get_offline_messages_calls_counts);
            Assert.AreEqual(1, remove_calls_counts);

            Assert.AreEqual(5, messages_sent_from_dialog.Count);
            //TestDialogMessage(messages_sent_from_dialog[4], last_message_number, TypeOfDialogMessage.OfflineMessages);

            TestCommonAuthDialogVariables();
            TestSpecialVariables(5, 0, last_message_sent_to_dialog, last_message_received_from_dialog_id, new List<DialogMessageForServer>(), DialogStatus.End, new ExceptionOccurenceStruct());

        }*/

        public void SendToDialog(TypeOfDialogMessage message_type, object message_object)
        {
            last_message_sent_to_dialg_id = GetGuid();
            last_message_number++;
            if (message_type != TypeOfDialogMessage.ReceiptMessage)
            {
                last_message_sent_to_dialog = CreateDialogMessage(message_type, last_message_sent_to_dialg_id, message_object, last_message_number);
                test_dialog.ReceiveMessage(last_message_sent_to_dialog);
            }
            else
            {
                test_dialog.ReceiveMessage(CreateDialogMessage(message_type, last_message_sent_to_dialg_id, message_object, last_message_number));
            }
        }

        /*
        public void SendToDialogCacheFormalMessage(TypeOfDialogMessage message_type, object message_object, int p_last_message_number)
        {
            last_message_sent_to_dialg_id = GetGuid();
            if (p_last_message_number == last_message_number + 1)
                last_message_number++;
            if (message_type != TypeOfDialogMessage.ReceiptMessage)
            {
                last_message_sent_to_dialog = CreateDialogMessage(message_type, last_message_sent_to_dialg_id, message_object, last_message_number);
                test_dialog.ReceiveMessage(last_message_sent_to_dialog);
            }
            else
            {
                test_dialog.ReceiveMessage(CreateDialogMessage(message_type, last_message_sent_to_dialg_id, message_object, last_message_number));
            }
        }*/

        public void TestSpecialVariables(int current_level, int p_retry_counts, DialogMessageForServer last_message_received,
            int last_message_sent_id, List<DialogMessageForServer> cache, DialogStatus status, ExceptionOccurenceStruct exception_occurance)
        {
            //Assert.AreEqual(last_message_number, test_dialog.Get_last_message_number);
            Assert.AreEqual(current_level, test_dialog.Get_current_level);
            Assert.AreEqual(last_message_received, test_dialog.Get_last_message_received);
            Assert.AreEqual(p_retry_counts, test_dialog.Get_retry_counts);

            Assert.AreEqual(test_dialog.Get_level_counts, test_dialog.Get_all_dialog_levels.Count);
            for (int i = 1; i < test_level_counts; i++)
            {
                bool flag = false;
                for (int j = 1; j < current_level; j++)
                {
                    Assert.AreEqual(test_dialog.Get_all_dialog_levels[j].Set_Get_level_executed, true);
                    flag = true;
                }
                if (flag == true)
                {
                    flag = false;
                    continue;
                }
                Assert.AreEqual(test_dialog.Get_all_dialog_levels[i].Set_Get_level_executed, false);
            }

            /*Assert.AreEqual(all_dialog_levels.Count, dialog.Get_all_dialog_levels.Count);
            foreach (KeyValuePair<int, bool> dialog_levels in all_dialog_levels)
            {
                Assert.AreEqual(dialog.Get_all_dialog_levels[dialog_levels.Key], dialog_levels.Value);
            }*/

            Assert.AreEqual(last_message_sent_id, test_dialog.Get_last_message_sent_id);

            Assert.AreEqual(cache.Count, test_dialog.Get_cache.Count);
            for (int i = 0; i < cache.Count; i++)
            {
                Assert.AreEqual(cache[i], test_dialog.Get_cache[i]);
            }

            Assert.AreEqual(status, test_dialog.Get_status);

            Assert.AreEqual(exception_occurance.Get_exception_message.Count, test_dialog.Get_exception_occurance.Get_exception_message.Count);
            Assert.AreEqual(exception_occurance.Get_exception_occured, test_dialog.Get_exception_occurance.Get_exception_occured);
            for (int i = 0; i < exception_occurance.Get_exception_message.Count; i++)
            {
                Assert.AreEqual(exception_occurance.Get_exception_message[i].Equals(test_dialog.Get_exception_occurance.Get_exception_message[i]), true);
            }

        }

        public void TestCommonUnauAuthDialogVariables()
        {
            Se_UnAuthDialog test_unauth_dialog = (Se_UnAuthDialog)test_dialog;

            Assert.AreEqual(test_thread_id, test_unauth_dialog.Get_thread_id);

            TestCommonVariables();
        }
        public void TestCommonAuthDialogVariables()
        {
            Se_AuthDialog test_auth_dialog = (Se_AuthDialog)test_dialog;

            Assert.AreEqual(test_user_name, test_auth_dialog.Get_user_name);

            TestCommonVariables();
        }

        public void TestCommonVariables()
        {
            Assert.AreEqual(test_dialog_id, test_dialog.Get_dialog_id);
            Assert.AreEqual(test_max_retry_counts, test_dialog.Get_max_retry_count);
            Assert.AreEqual(test_dialog_type, test_dialog.Get_dialog_type);
            Assert.AreEqual(test_level_counts, test_dialog.Get_level_counts);
            Assert.AreEqual(test_level_counts, test_dialog.Get_all_dialog_levels.Count);

            Assert.AreEqual(last_message_number, test_dialog.Get_last_message_number);

        }

        public void TestLockToBeeFree(ReaderWriterLockSlim input_lock)
        {
            Assert.AreEqual(false, input_lock.IsReadLockHeld);
            Assert.AreEqual(false, input_lock.IsUpgradeableReadLockHeld);
            Assert.AreEqual(false, input_lock.IsWriteLockHeld);

        }

        public void TestDialogMessage(DialogMessageForServer message, int message_number_in_dialog, TypeOfDialogMessage message_object_type)
        {
            Assert.AreEqual(test_dialog_id, message.Get_dialog_id);
            Assert.AreEqual(test_dialog_type, message.Get_dialog_type);
            Assert.AreEqual(message_number_in_dialog, message.Get_message_number_in_dialog);
            Assert.AreEqual(message_object_type, message.Get_message_object_type);
            Assert.IsTrue(DialogMessageObjectInvestigate(message));
            Assert.AreEqual(typeof(int), message.Get_message_id.GetType());

            if (message.Get_message_object_type == TypeOfDialogMessage.ReceiptMessage)
            {
                Di_Mess_ReceiptMessage temp_receipt_object = (Di_Mess_ReceiptMessage)message.Get_message_object;
                if (temp_receipt_object.Get_message_rec_status == ReceiptStatus.Accepted)
                {
                    Di_Mess_Rec_AcceptMessage temp_accept_receipt_object = ((Di_Mess_Rec_AcceptMessage)temp_receipt_object.Get_rec_message);
                    Assert.AreEqual(last_message_sent_to_dialg_id, temp_accept_receipt_object.Get_accpepted_message_id);
                }
                else if (temp_receipt_object.Get_message_rec_status == ReceiptStatus.Rejected)
                {
                    Di_Mess_Rec_RejectMessage temp_reject_receipt_messsage = ((Di_Mess_Rec_RejectMessage)temp_receipt_object.Get_rec_message);
                    Assert.AreEqual(last_message_sent_to_dialg_id, temp_reject_receipt_messsage.Get_rejected_message_id);
                    Assert.AreNotEqual(null, temp_reject_receipt_messsage.Get_comment_for_rejecting);
                }
            }
            else
            {
            }

        }

        public bool DialogMessageObjectInvestigate(DialogMessageForServer p_dialog_message)
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
                }   //19 ta

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
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public int GetGuid()
        {
            Guid p_guid = Guid.NewGuid();
            byte[] me = p_guid.ToByteArray();
            return BitConverter.ToInt32(me, 0);
        }

        public void UnAuthSend(int thread_id, DialogMessageForClient message)
        {
            last_message_received_from_dialog_id = message.Get_message_id;
            //Assert.AreEqual(test_thread_id, thread_id);
            messages_sent_from_dialog.Add(message);
            last_message_number++;
            unauth_send_calls_count++;
        }
        public void AuthSend(string user_name, DialogMessageForClient message)
        {
            last_message_received_from_dialog_id = message.Get_message_id;
            //Assert.AreEqual(test_user_name, user_name);
            messages_sent_from_dialog.Add(message);
            last_message_number++;
            auth_send_calls_counts++;
        }
        public List<string> GetFriendsList(string user_name)
        {
            List<string> friends_list = new List<string>();
            friends_list.Add("ali");
            friends_list.Add("ahmad");
            friends_list.Add("hasan");
            friends_list.Add("reza");
            get_friends_list_calls_counts++;
            return friends_list;
        }
        public UserStatus GetUserStatus(string user_name)
        {
            get_user_status_calls_counts++;
            return UserStatus.Online;
        }
        public List<int> GetPublicChatsIds()
        {
            List<int> public_chat_ids = new List<int>();
            public_chat_ids.Add(2);
            public_chat_ids.Add(5);
            public_chat_ids.Add(43);
            public_chat_ids.Add(354345);
            public_chat_ids.Add(35345);
            public_chat_ids.Add(5567567);
            get_public_chat_ids_calls_count++;
            return public_chat_ids;
        }
        public List<OfflineMessage> GetOfflineMessages(string user_name)
        {
            List<OfflineMessage> offline_messages = new List<OfflineMessage>();
            offline_messages.Add(new OfflineMessage(HelperFunctions.GetGUID(), "admin", "hhh"));
            offline_messages.Add(new OfflineMessage(HelperFunctions.GetGUID(), "ali", "iii"));
            get_offline_messages_calls_counts++;
            return offline_messages;
        }
        public List<AgreementInvitationInfo> GetAgreementInvitation(string user_name)
        {
            List<AgreementInvitationInfo> agreements_list = new List<AgreementInvitationInfo>();
            agreements_list.Add(new AgreementInvitationInfo("ali", "bia", 23, TypeOfAgreement.Add));
            agreements_list.Add(new AgreementInvitationInfo("ahmad", "naia", 44, TypeOfAgreement.Add));
            agreements_list.Add(new AgreementInvitationInfo("reza", "boro", 63, TypeOfAgreement.Add));
            get_agreement_invitation_calls_counts++;
            return agreements_list;
        }

        public Se_BaseBooleanFunctionResult GetPrivateChatInvitationAnswer(string p_user_name, int p_chat_id, bool p_answer)
        {
            get_private_chat_invitation_answer_calls_count++;
            return new Se_BooleanFunctionAccResult();
        }
        public void OfflineMessagesReadInform(string p_user_name, List<int> p_message_ids)
        {
            offline_messages_read_inform_calls_count++;
            return;
        }
        public void GetAgreementAnswer(string p_user_name, int p_agreement_id, bool p_answer, TypeOfAgreement p_agreement_type)
        {
            get_agreement_answer_calls_count++;
            return;
        }
        public Se_BaseBooleanFunctionResult SignUp(string p_user_name, string p_password)
        {
            signup_calls_count++;
            return new Se_BooleanFunctionAccResult();
        }
        public bool IstherUser(string p_user_name)
        {
            is_there_user_calls_count++;
            return true;
        }
        public void CreateFormalMessageRequest(FormalMessage p_message, string p_receiver_user_name)
        {
            create_formal_message_request_calls_count++;
            return;
        }
        public List<string> GetPublicChatUsersList(int p_chat_id)
        {
            get_public_chat_users_list_calls_count++;
            List<string> users_list = new List<string>();
            users_list.Add("ahmad");
            users_list.Add("ali");
            users_list.Add("farhad");
            users_list.Add("ali");
            return users_list;
        }
        public bool IsUserInPublicChat(string p_user_name, int p_public_chat_id)
        {
            is_user_in_public_chat_calls_count++;
            return false;
        }
        public Se_BaseBooleanFunctionResult JoinPublicChatRequest(string p_user_name, int p_chat_id)
        {
            join_public_chat_request_calls_count++;
            return new Se_BooleanFunctionAccResult();
        }
        public void StartClientCreatedPrivateChatInform(string p_user_name, string p_starter_user_name, int p_chat_id)
        {
            start_client_created_private_chat_inform_calls_count++;
            return;
        }
        public bool IsTherePrivateChat(int p_private_chat_id)
        {
            is_there_private_chat_calls_count++;
            return false;
        }
        public Se_BaseIntFunctionResult CreatePrivateChat(string p_first_person_user_name, string p_second_person_user_name)
        {
            create_private_chat_calls_count++;
            return new Se_IntFunctionAccResult(20);
        }
        public Se_BaseBooleanFunctionResult OnlineAreFriends(string p_first_person_user_name, string p_second_person_user_name)
        {
            online_are_friends_calls_count++;
            return new Se_BooleanFunctionAccResult();
        }
        public void ServerInformChatLeave(string p_user_name, int p_chat_id, TypeOfChat p_chat_type)
        {
            server_inform_chat_leave_calls_count++;
            return;
        }

        public Se_BaseBooleanFunctionResult CreateAddAgreement(string p_starter_user_name, string p_invited_user_id)
        {
            create_add_agreement_calls_count++;
            return new Se_BooleanFunctionAccResult();
        }
        public void CreateOfflineMessage(string p_user_name, OfflineMessage p_message)
        {
            create_offline_message_calls_count++;
        }

        public Se_BaseBooleanFunctionResult Login(int p_thread_id, string user_name, string password)
        {
            login_calls_count++;
            return new Se_BooleanFunctionAccResult();
        }
        public bool IsThereUnauthWorkerThread(int thread_id)
        {
            is_there_unauth_worker_thread_calls_count++;
            return true;
        }

        public void Remove(int dialog_id)
        {
            remove_calls_counts++;
        }
        public bool IsLoggedIn(string user_name)
        {
            is_logged_in_calls_counts++;
            return true;
        }

        public DialogMessageForServer CreateDialogMessage(TypeOfDialogMessage message_object_type, int message_id, object message_object, int message_number)
        {
            return new DialogMessageForServer(message_id, test_dialog_id, message_number, test_dialog_type, message_object, message_object_type);
        }


        private static Dictionary<TypeOfDialog, Dictionary<int, Se_AuthDialog>> CreateAllAuthDialogs()
        {
            Dictionary<int, Se_AuthDialog> login_data_request_dialogs = new Dictionary<int, Se_AuthDialog>();
            Dictionary<int, Se_AuthDialog> create_add_agreement_dialogs = new Dictionary<int, Se_AuthDialog>();
            Dictionary<int, Se_AuthDialog> get_agreement_answer_dialogs = new Dictionary<int, Se_AuthDialog>();
            Dictionary<int, Se_AuthDialog> server_read_offline_message_dialog_dialogs = new Dictionary<int, Se_AuthDialog>();
            Dictionary<int, Se_AuthDialog> client_leave_chat_request_dialogs = new Dictionary<int, Se_AuthDialog>();
            Dictionary<int, Se_AuthDialog> client_someone_left_chat_inform_dialogs = new Dictionary<int, Se_AuthDialog>();
            Dictionary<int, Se_AuthDialog> inform_ejected_chat_user_dialogs = new Dictionary<int, Se_AuthDialog>();
            Dictionary<int, Se_AuthDialog> create_private_chat_request_dialogs = new Dictionary<int, Se_AuthDialog>();
            Dictionary<int, Se_AuthDialog> client_join_public_chat_request_dialogs = new Dictionary<int, Se_AuthDialog>();
            Dictionary<int, Se_AuthDialog> client_someone_joined_chat_inform_dialogs = new Dictionary<int, Se_AuthDialog>();
            Dictionary<int, Se_AuthDialog> client_friend_changed_status_inform_dialogs = new Dictionary<int, Se_AuthDialog>();
            Dictionary<int, Se_AuthDialog> send_to_client_formal_message_dialogs = new Dictionary<int, Se_AuthDialog>();
            Dictionary<int, Se_AuthDialog> formal_message_request_dialogs = new Dictionary<int, Se_AuthDialog>();
            Dictionary<int, Se_AuthDialog> client_friend_list_changed_inform_dialogs = new Dictionary<int, Se_AuthDialog>();
            Dictionary<int, Se_AuthDialog> client_created_private_chat_inform_dialogs = new Dictionary<int, Se_AuthDialog>();
            Dictionary<int, Se_AuthDialog> client_invited_agreement_inform_dialog_manager = new Dictionary<int, Se_AuthDialog>();

            Dictionary<TypeOfDialog, Dictionary<int, Se_AuthDialog>> all_auth_dialogs = new Dictionary<TypeOfDialog, Dictionary<int, Se_AuthDialog>>();
            all_auth_dialogs.Add(TypeOfDialog.LoginDataRequest, login_data_request_dialogs);
            all_auth_dialogs.Add(TypeOfDialog.CreateAddAgreement, create_add_agreement_dialogs);
            all_auth_dialogs.Add(TypeOfDialog.GetAgreementAnswer, get_agreement_answer_dialogs);
            all_auth_dialogs.Add(TypeOfDialog.ServerReadOfflineMessagesInform, server_read_offline_message_dialog_dialogs);
            all_auth_dialogs.Add(TypeOfDialog.ClientLeaveChatRequest, client_leave_chat_request_dialogs);
            all_auth_dialogs.Add(TypeOfDialog.ClientSomeoneLeftChatInform, client_someone_left_chat_inform_dialogs);
            all_auth_dialogs.Add(TypeOfDialog.InformEjectedChatUser, inform_ejected_chat_user_dialogs);
            all_auth_dialogs.Add(TypeOfDialog.CreatePrivateChatRequest, create_private_chat_request_dialogs);
            all_auth_dialogs.Add(TypeOfDialog.ClientJoinPublicChatRequest, client_join_public_chat_request_dialogs);
            all_auth_dialogs.Add(TypeOfDialog.ClientSomeoneJoinedChatInform, client_someone_joined_chat_inform_dialogs);
            all_auth_dialogs.Add(TypeOfDialog.ClientFriendChangedStatusInform, client_friend_changed_status_inform_dialogs);
            all_auth_dialogs.Add(TypeOfDialog.SendToClinetFormalMessage, send_to_client_formal_message_dialogs);
            all_auth_dialogs.Add(TypeOfDialog.FormalMessageRequest, formal_message_request_dialogs);
            all_auth_dialogs.Add(TypeOfDialog.ClientFriendListChangedInform, client_friend_list_changed_inform_dialogs);
            all_auth_dialogs.Add(TypeOfDialog.ClientCreatedPrivateChatInform, client_created_private_chat_inform_dialogs);
            all_auth_dialogs.Add(TypeOfDialog.ClientInvitedAgreementInform, client_invited_agreement_inform_dialog_manager);

            return all_auth_dialogs;  
        }
        private static Dictionary<TypeOfDialog, Dictionary<int, Se_UnAuthDialog>> CreateAllUnAuthDialogs()
        {
            Dictionary<TypeOfDialog, Dictionary<int, Se_UnAuthDialog>> all_unauth_dialogs = new Dictionary<TypeOfDialog, Dictionary<int, Se_UnAuthDialog>>();
            Dictionary<int, Se_UnAuthDialog> login_equest_dialogs = new Dictionary<int, Se_UnAuthDialog>();
            Dictionary<int, Se_UnAuthDialog> client_signup_request_dialogs = new Dictionary<int, Se_UnAuthDialog>();
            all_unauth_dialogs.Add(TypeOfDialog.LoginRequest, login_equest_dialogs);
            all_unauth_dialogs.Add(TypeOfDialog.ClientSignupRequest, client_signup_request_dialogs);
            return all_unauth_dialogs;
        }

        public AgreementInvitationInfo Test_GetAUserAgreementtInvitation(string username, int id)
        {
            AgreementInvitationInfo T_Agreementinvitationinfo = new AgreementInvitationInfo("Ali", "hello", 3, TypeOfAgreement.Add);
            return T_Agreementinvitationinfo;
        }

    }

    /*[TestClass]
    public class Test_Di_Server_LoginRequest
    {

        List<DialogMessage> dialog_sent_messages;

        [TestMethod]
        public void Test_FirstPath_Di_Server_LoginRequest()
        {
            Random rn = new Random();
            int random_dialog_id = rn.Next();
            int random_thread_id = rn.Next();
            dialog_sent_messages = new List<DialogMessage>();

            Di_Server_LoginRequest test_login_request_dialog = new Di_Server_LoginRequest(random_dialog_id, random_thread_id, new Remove(TestHelpFunctions.Remove), new Login(TestHelpFunctions.Login), new UnAuthSend(UnAuthSend), new IsThereUnauthWorkerThread(TestHelpFunctions.IsThereUnauthThread));

            Assert.AreEqual(1, test_login_request_dialog.Get_all_dialog_levels.Count);

            Assert.AreEqual(false, test_login_request_dialog.Get_all_dialog_levels[1].Set_Get_level_executed);
            Assert.AreEqual(DialogLevelType.WaitingForMessageReceive, test_login_request_dialog.Get_all_dialog_levels[1].Get_level_type);
            Assert.AreEqual(0, test_login_request_dialog.Get_cache.Count);
            Assert.AreEqual(1, test_login_request_dialog.Get_current_level);
            Assert.AreEqual(random_dialog_id, test_login_request_dialog.Get_dialog_id);
            Assert.AreEqual(TypeOfDialog.LoginRequest, test_login_request_dialog.Get_dialog_type);
            Assert.AreEqual(0, test_login_request_dialog.Get_last_message_number);
            Assert.AreEqual(null, test_login_request_dialog.Get_last_message_received);
            Assert.AreEqual(0, test_login_request_dialog.Get_last_message_sent_id);
            Assert.AreEqual(1, test_login_request_dialog.Get_level_counts);
            Assert.AreEqual(DialogStatus.WaitingForAMessage, test_login_request_dialog.Get_status);
            Assert.AreEqual(random_thread_id, test_login_request_dialog.Get_thread_id);

            Di_Mess_LoginRequestData test_login_data_object = new Di_Mess_LoginRequestData("test", "test");
            DialogMessage test_dialog_message = new DialogMessage(rn.Next(), random_dialog_id, 1, TypeOfDialog.LoginRequest, test_login_data_object, TypeOfDialogMessage.LoginRequestData);
            test_login_request_dialog.ReceiveMessage(test_dialog_message);

            //level One after message arrived:
            Assert.AreEqual(1, test_login_request_dialog.Get_all_dialog_levels.Count);

            Assert.AreEqual(true, test_login_request_dialog.Get_all_dialog_levels[1].Set_Get_level_executed);
            Assert.AreEqual(DialogLevelType.WaitingForMessageReceive, test_login_request_dialog.Get_all_dialog_levels[1].Get_level_type);
            Assert.AreEqual(0, test_login_request_dialog.Get_cache.Count);
            Assert.AreEqual(1, test_login_request_dialog.Get_current_level);
            Assert.AreEqual(random_dialog_id, test_login_request_dialog.Get_dialog_id);
            Assert.AreEqual(TypeOfDialog.LoginRequest, test_login_request_dialog.Get_dialog_type);
            Assert.AreEqual(2, test_login_request_dialog.Get_last_message_number);
            Assert.AreEqual(test_dialog_message, test_login_request_dialog.Get_last_message_received);
            Assert.AreEqual(test_dialog_message.Get_dialog_id, test_login_request_dialog.Get_dialog_id);
            Assert.AreEqual(1, test_login_request_dialog.Get_level_counts);
            Assert.AreEqual(DialogStatus.End, test_login_request_dialog.Get_status);
            Assert.AreEqual(random_thread_id, test_login_request_dialog.Get_thread_id);
            //Assert.AreNotEqual(0, test_login_request_dialog.Get_last_message_number);

            Assert.AreEqual(1, dialog_sent_messages.Count);
            Assert.AreEqual(TypeOfDialog.LoginRequest, dialog_sent_messages[0].Get_dialog_type);
            Assert.AreEqual(random_dialog_id, dialog_sent_messages[0].Get_dialog_id);
            Assert.AreEqual(2, dialog_sent_messages[0].Get_message_number_in_dialog);
            Assert.AreEqual(TypeOfDialogMessage.ReceiptMessage, dialog_sent_messages[0].Get_message_object_type);

            Di_Mess_ReceiptMessage test_receipt_message = null;
            try
            {
                test_receipt_message = (Di_Mess_ReceiptMessage)dialog_sent_messages[0].Get_message_object;
            }
            catch (Exception Ex)
            {
                Assert.Fail(Ex.Message);
            }
            if (test_receipt_message.Get_message_rec_status == ReceiptStatus.Accepted)
            {
                Di_Mess_Rec_AcceptMessage test_accept_object = (Di_Mess_Rec_AcceptMessage)test_receipt_message.Get_rec_message;
                Assert.AreEqual(test_dialog_message.Get_message_id, test_accept_object.Get_accpepted_message_id);
            }
            else if (test_receipt_message.Get_message_rec_status == ReceiptStatus.Rejected)
            {
                Di_Mess_Rec_RejectMessage test_reject_object = (Di_Mess_Rec_RejectMessage)test_receipt_message.Get_rec_message;
                Assert.AreEqual(test_dialog_message.Get_dialog_id, test_reject_object.Get_rejected_message_id);
            }






        }

        public bool UnAuthSend(int p_thead_id, DialogMessage p_message)
        {
            dialog_sent_messages.Add(p_message);
            return true;
        }



    }*/

    public static class TestHelpFunctions
    {
        public static DialogMessageForServer CreateAccMessage(DialogMessageForServer p_message)
        {
            Random rn = new Random();
            Di_Mess_Rec_AcceptMessage acc_object = new Di_Mess_Rec_AcceptMessage(p_message.Get_message_id);
            Di_Mess_ReceiptMessage rec_message_object = new Di_Mess_ReceiptMessage(ReceiptStatus.Accepted, acc_object);
            DialogMessageForServer acc_message = new DialogMessageForServer(rn.Next(), p_message.Get_dialog_id, p_message.Get_message_number_in_dialog + 1, p_message.Get_dialog_type, rec_message_object, TypeOfDialogMessage.ReceiptMessage);
            return acc_message;

        }
        public static DialogMessageForServer CreateARejMessage(DialogMessageForServer p_message, string p_reason)
        {
            Random rn = new Random();
            Di_Mess_Rec_RejectMessage rej_object = new Di_Mess_Rec_RejectMessage(p_message.Get_message_id, p_reason);
            Di_Mess_ReceiptMessage rec_message_object = new Di_Mess_ReceiptMessage(ReceiptStatus.Rejected, rej_object);
            DialogMessageForServer acc_message = new DialogMessageForServer(rn.Next(), p_message.Get_dialog_id, p_message.Get_message_number_in_dialog + 1, p_message.Get_dialog_type, rec_message_object, TypeOfDialogMessage.ReceiptMessage);
            return acc_message;
        }

        public static List<AgreementInvitationInfo> Get_users_agreement_invitation(string p_user_name)
        {
            Random rn = new Random();
            List<AgreementInvitationInfo> list_for_send = new List<AgreementInvitationInfo>();
            list_for_send.Add(new AgreementInvitationInfo("ahmad", "what???", rn.Next(), TypeOfAgreement.Add));
            list_for_send.Add(new AgreementInvitationInfo("ali", "what???", rn.Next(), TypeOfAgreement.Add));
            list_for_send.Add(new AgreementInvitationInfo("mohammad", "what???", rn.Next(), TypeOfAgreement.Add));
            list_for_send.Add(new AgreementInvitationInfo("hasan", "what???", rn.Next(), TypeOfAgreement.Add));
            list_for_send.Add(new AgreementInvitationInfo("mahmoood", "what???", rn.Next(), TypeOfAgreement.Add));
            list_for_send.Add(new AgreementInvitationInfo("reza", "what???", rn.Next(), TypeOfAgreement.Add));
            return list_for_send;
        }

        public static List<OfflineMessage> Get_user_offline_messages(string p_user_name)
        {
            List<OfflineMessage> all_user_offline_messages = new List<OfflineMessage>();
            all_user_offline_messages.Add(new OfflineMessage(HelperFunctions.GetGUID(), "admin", "test"));
            all_user_offline_messages.Add(new OfflineMessage(HelperFunctions.GetGUID(), "ahmad", "test"));
            all_user_offline_messages.Add(new OfflineMessage(HelperFunctions.GetGUID(), "ali", "test"));
            all_user_offline_messages.Add(new OfflineMessage(HelperFunctions.GetGUID(), "farhad", "test"));
            all_user_offline_messages.Add(new OfflineMessage(HelperFunctions.GetGUID(), "javad", "test"));
            all_user_offline_messages.Add(new OfflineMessage(HelperFunctions.GetGUID(), "davood", "test"));
            return all_user_offline_messages;
        }

        public static List<int> Get_public_chat_ids()
        {
            Random rn = new Random();
            List<int> chat_ids = new List<int>();
            for (int i = 0; i < 5; i++)
            {
                chat_ids.Add(rn.Next());
            }
            return chat_ids;
        }

        public static UserStatus Get_user_status(string p_user_name)
        {
            Random rn = new Random();
            int gg = rn.Next();
            if (gg % 2 == 0)
            {
                return UserStatus.Online;
            }
            else
            {
                return UserStatus.Offline;
            }
        }

        public static List<string> Get_all_user_friends(string p_user_name)
        {
            List<string> all_friend_list = new List<string>();
            all_friend_list.Add("ahmad");
            all_friend_list.Add("ali");
            all_friend_list.Add("reza");
            all_friend_list.Add("hasan");
            all_friend_list.Add("mahmood");
            all_friend_list.Add("davood");
            return all_friend_list;
        }

        public static bool IsLoggeIn(string p_user_name)
        {
            Random rn = new Random();
            int gg = rn.Next();
            if (gg % 2 == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool AuuthSend(string p_user_name, DialogMessageForServer p_message)
        {
            bool seneded = true;
            return true;
        }

        public static void Remove(int p_dialog_id)
        {
            bool removed = true;
        }

        public static bool IsThereUnauthThread(int p_thread_id)
        {
            return true;
        }

        public static Se_BaseBooleanFunctionResult Login(string p_user_name, string p_pass_word)
        {
            /*Random rn = new Random();
            int gg = rn.Next();
            if (gg % 2 == 0)
            {*/
            return new Se_BooleanFunctionAccResult();
            /*}
            else
            {
                return new Se_BooleanFunctionRejResult("Some Reasons");
            }*/
        }
    }
}
