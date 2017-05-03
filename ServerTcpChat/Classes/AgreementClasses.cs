using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonChatTypes;
using System.Threading;

namespace ServerTcpChat.Classes
{
    public delegate void DB_RemoveAgreement(int p_agreement_id);
    public delegate void DB_CreateAddAgreement(string p_starter_user_name, string p_user_to_add_name, int p_agreement_id);
    public delegate bool DB_AreFriends(string p_first_person_user_name, string p_second_person_user_name);
    public delegate void DB_AddToFriends(string p_first_person_user_name, string p_second_person_user_name);
    public delegate bool DB_IsThereUser(string p_user_name);
    public delegate bool DB_IsThereAgreement(int p_agreement_id);

    public delegate void StartClientFriendListChangedInformDialog(string p_user_name, List<PersonStatus> p_friends_and_status);
    public delegate void ReloadFriendList(string p_user_name);
    public delegate List<string> GetOnlineUserFriendList(string p_user_name);

    public delegate void AddAgreementDone(int p_agreement_id);

    public delegate List<Agreement> GetAllAdAgreements();

    public delegate void RemoveAgr(int p_agreement_id);


    public abstract class BaseAgreement
    {
        protected RemoveAgr remove_agreement_from_manager;
        protected CreateFormalMessageRequest send_formal_message_to_user;
        protected DB_IsThereUser is_there_user;
        protected IsLoggedIn is_logged_in;

        protected Dictionary<string, bool> all_users_invited;
        protected int max_invited_persons;
        protected TypeOfAgreement agreement_type;
        protected string starter_user_name;
        protected int agreement_id;

        protected void BaseConstruct(TypeOfAgreement p_agreement_type, int p_agreement_id, string p_starter_user_name
            , int p_max_invited_persons, RemoveAgr p_remove_agreement_from_manager, CreateFormalMessageRequest p_send_formal_message_to_user
            , DB_IsThereUser p_is_there_user, Dictionary<string, bool> p_all_users_invited, IsLoggedIn p_is_logged_in)
        {
            all_users_invited = p_all_users_invited;

            agreement_id = p_agreement_id;
            max_invited_persons = p_max_invited_persons;
            agreement_type = p_agreement_type;
            starter_user_name = p_starter_user_name;

            remove_agreement_from_manager = p_remove_agreement_from_manager;
            send_formal_message_to_user = p_send_formal_message_to_user;
            is_there_user = p_is_there_user;
            is_logged_in = p_is_logged_in;
        }

        public abstract void ReceiveAnswer(string p_user_name, bool p_answer);

        protected abstract void Result();

        protected abstract void Fail();

        public void RemoveAgreement()
        {
            remove_agreement_from_manager(agreement_id);
        }

    }

    public class AddAgreement : BaseAgreement
    {
        DB_AddToFriends add_to_friends;
        DB_AreFriends are_friends;
        DB_CreateAddAgreement create_add_agreement;
        DB_RemoveAgreement remove_agreement;
        GetUserStatus get_user_status;
        StartClientFriendListChangedInformDialog start_client_friend_list_changed_inform_dialog;
        ReloadFriendList reload_friend_list;
        GetOnlineUserFriendList get_online_user_friend_list;
        AddAgreementDone add_agreement_done;

        public AddAgreement(int p_agreement_id, string p_starter_user_name, string p_invited_person_user_name, RemoveAgr p_remove_agreement_from_manager
            , DB_AddToFriends p_add_to_friends, DB_AreFriends p_are_friends, DB_CreateAddAgreement p_create_add_agreement
            , DB_RemoveAgreement p_remove_agreement, CreateFormalMessageRequest p_send_formal_message_to_user, DB_IsThereUser p_is_there_user
            , GetUserStatus p_get_user_status, IsLoggedIn p_is_logged_in, StartClientFriendListChangedInformDialog p_start_client_friend_list_changed_inform_dialog
            , ReloadFriendList p_reload_friend_list, GetOnlineUserFriendList p_get_online_user_friend_list, AddAgreementDone p_add_agreement_done)
        {
            Dictionary<string, bool> t_all_users_invited = new Dictionary<string, bool>();
            t_all_users_invited.Add(p_invited_person_user_name, false);

            base.BaseConstruct(TypeOfAgreement.Add, p_agreement_id, p_starter_user_name, 1, p_remove_agreement_from_manager, p_send_formal_message_to_user, p_is_there_user
                , t_all_users_invited, p_is_logged_in);

            add_to_friends = p_add_to_friends;
            are_friends = p_are_friends;
            create_add_agreement = p_create_add_agreement;
            remove_agreement = p_remove_agreement;
            get_user_status = p_get_user_status;
            start_client_friend_list_changed_inform_dialog = p_start_client_friend_list_changed_inform_dialog;
            reload_friend_list = p_reload_friend_list;
            get_online_user_friend_list = p_get_online_user_friend_list;
            get_user_status = p_get_user_status;
            add_agreement_done = p_add_agreement_done;
        }

        public override void ReceiveAnswer(string p_user_name, bool p_answer)
        {
            List<string> list_all_users_invited = all_users_invited.Keys.ToList();
            if (list_all_users_invited[0] == p_user_name)
            {
                if (p_answer == true)
                {
                    Result();
                }
                else if (p_answer == false)
                {
                    Fail();
                }
            }
        }

        protected override void Result()
        {

            List<string> all_users_invited_names = all_users_invited.Keys.ToList();
            FormalMessage agreement_accepted_message = new FormalMessage("Admin", "your request for adding " + all_users_invited_names[0] + " was Accepted");

            add_agreement_done(agreement_id);

            int j = 0;
            while (j < 4)
            {
                try
                {
                    send_formal_message_to_user(agreement_accepted_message, starter_user_name);
                    break;
                }
                catch (Exception)
                {
                    j++;
                    continue;
                }
            }


            if (is_logged_in(starter_user_name))
            {
                try
                {
                    reload_friend_list(starter_user_name);
                    List<string> new_friend_list = get_online_user_friend_list(starter_user_name);
                    List<PersonStatus> new_friend_list_and_status = new List<PersonStatus>();
                    foreach (string t_friend in new_friend_list)
                    {
                        new_friend_list_and_status.Add(new PersonStatus(t_friend, get_user_status(t_friend)));
                    }
                    start_client_friend_list_changed_inform_dialog(starter_user_name, new_friend_list_and_status);
                }
                catch (Exception)
                {
                }
            }
            if (is_logged_in(all_users_invited_names[0]))
            {
                try
                {
                    reload_friend_list(all_users_invited_names[0]);
                    List<string> new_friend_list = get_online_user_friend_list(all_users_invited_names[0]);
                    List<PersonStatus> new_friend_list_and_status = new List<PersonStatus>();
                    foreach (string t_friend in new_friend_list)
                    {
                        new_friend_list_and_status.Add(new PersonStatus(t_friend, get_user_status(t_friend)));
                    }
                    start_client_friend_list_changed_inform_dialog(all_users_invited_names[0], new_friend_list_and_status);
                }
                catch (Exception)
                {
                }
            }

            RemoveAgreement();
            return;
        }

        protected override void Fail()
        {
            remove_agreement(agreement_id);
            List<string> all_users_invited_names = all_users_invited.Keys.ToList();
            FormalMessage agreement_failed_message = new FormalMessage("Admin", "your request for adding " + all_users_invited_names[0] + " was rejected");
            int j = 0;
            while (j < 4)
            {
                try
                {
                    send_formal_message_to_user(agreement_failed_message, starter_user_name);
                    break;
                }
                catch (Exception)
                {
                    j++;
                    continue;
                }
            }
            RemoveAgreement();
            return;
        }

        public override bool Equals(object obj)
        {
            try
            {
                AddAgreement add_agreement = (AddAgreement)obj;
                if ((add_agreement.starter_user_name == starter_user_name && add_agreement.all_users_invited.ElementAt(0).Key == all_users_invited.ElementAt(0).Key)
                    || (add_agreement.all_users_invited.ElementAt(0).Key == starter_user_name && add_agreement.starter_user_name == all_users_invited.ElementAt(0).Key))
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }

    public abstract class BaseAgreementManager
    {
        protected CreateFormalMessageRequest send_formal_message_to_user;
        protected DB_IsThereUser is_there_user;
        protected StartClientInvitedAgreementInform start_client_invited_agreement_inform;
        protected IsLoggedIn is_logged_in;

        protected int max_invited_persons;
        protected TypeOfAgreement agreement_type;


        public void BaseConstruct(TypeOfAgreement p_agreement_type, int p_max_invited_persons, CreateFormalMessageRequest p_send_formal_message_to_user
            , DB_IsThereUser p_is_there_user, StartClientInvitedAgreementInform p_start_client_invited_agreement_inform, IsLoggedIn p_is_logged_in)
        {
            agreement_type = p_agreement_type;
            send_formal_message_to_user = p_send_formal_message_to_user;
            start_client_invited_agreement_inform = p_start_client_invited_agreement_inform;
            is_logged_in = p_is_logged_in;
            is_there_user = p_is_there_user;
            max_invited_persons = p_max_invited_persons;
        }

        public abstract void GetAgreementAnswer(string p_user_name, int p_agreement_id, bool p_answer);

        public abstract void AgreementRemoveItselfRequest(int p_agreement_id);

        public void Ma_SendFormalMessageToUser(FormalMessage p_message, string p_receiver_user_name)
        {
            send_formal_message_to_user(p_message, p_receiver_user_name);
        }   
        public void Ma_StartClientInvitedAgreementInform(string p_user_name, int p_agreement_id)
        {
            start_client_invited_agreement_inform(p_user_name, p_agreement_id);
        }
        public bool Ma_IsLoggedIn(string p_user_name)
        {
            return is_logged_in(p_user_name);
        }

    }

    public class Ma_AddAgreementManager : BaseAgreementManager
    {
        Dictionary<int, AddAgreement> all_agreements;

        DB_AddToFriends add_to_friends;
        DB_AreFriends are_friends;
        DB_CreateAddAgreement create_add_agreement;
        DB_RemoveAgreement remove_agreement;

        StartClientFriendListChangedInformDialog start_client_friend_list_changed_inform_dialog;
        ReloadFriendList reload_friend_list;
        GetOnlineUserFriendList get_online_user_friend_list;
        GetUserStatus get_user_status;
        AddAgreementDone add_agreement_done;
        GetAllAdAgreements get_all_agreements;

        public Ma_AddAgreementManager(ref Dictionary<int, AddAgreement> p_all_agreements, CreateFormalMessageRequest p_send_formal_messages_to_user
            , DB_IsThereUser p_is_there_user, DB_AddToFriends p_add_to_friends, DB_AreFriends p_are_friends, DB_CreateAddAgreement p_create_add_agreement
            , DB_RemoveAgreement p_remove_agreement, IsLoggedIn p_is_logged_in, StartClientFriendListChangedInformDialog p_start_client_friend_list_changed_inform_dialog
            , ReloadFriendList p_reload_friend_list, GetOnlineUserFriendList p_get_online_user_friend_list, GetUserStatus p_get_user_status, AddAgreementDone p_add_agreement_done
            , GetAllAdAgreements p_get_all_agreements, StartClientInvitedAgreementInform p_start_client_invited_agreement_inform)
        {
            base.BaseConstruct(TypeOfAgreement.Add, 1, p_send_formal_messages_to_user, p_is_there_user, p_start_client_invited_agreement_inform, p_is_logged_in);

            add_to_friends = p_add_to_friends;
            are_friends = p_are_friends;
            create_add_agreement = p_create_add_agreement;
            remove_agreement = p_remove_agreement;
            start_client_friend_list_changed_inform_dialog = p_start_client_friend_list_changed_inform_dialog;
            reload_friend_list = p_reload_friend_list;
            get_online_user_friend_list = p_get_online_user_friend_list;
            get_user_status = p_get_user_status;
            add_agreement_done = p_add_agreement_done;
            get_all_agreements = p_get_all_agreements;

            all_agreements = p_all_agreements;

            List<Agreement> all_db_agreements = new List<Agreement>();
            int h = 0;
            while (h < 4)
            {
                try
                {
                    all_db_agreements = get_all_agreements();
                    h = 0;
                    break;
                }
                catch
                {
                    h++;
                    continue;
                }
            }
            if (h > 3)
            {
                Console.WriteLine("could not get all agreements from DB");
                try
                {
                    Environment.Exit(4);
                }
                catch
                {
                }
                Thread.CurrentThread.Abort();

            }
            foreach (Agreement t_agreement in all_db_agreements)
            {
                AddAgreement temp_add_agreement = new AddAgreement(t_agreement.Get_agreement_id, t_agreement.Get_inviting_user_name, t_agreement.Get_invited_user_name, new RemoveAgr(AgreementRemoveItselfRequest)
                , new DB_AddToFriends(add_to_friends), new DB_AreFriends(are_friends), new DB_CreateAddAgreement(create_add_agreement), new DB_RemoveAgreement(remove_agreement)
                , new CreateFormalMessageRequest(Ma_SendFormalMessageToUser), new DB_IsThereUser(is_there_user), new GetUserStatus(Ma_GetUserStatus), new IsLoggedIn(Ma_IsLoggedIn), new StartClientFriendListChangedInformDialog(Ma_StartClientFriendListChangedInformDialog)
                , new ReloadFriendList(Ma_ReloadFriendList), new GetOnlineUserFriendList(Ma_GetOnlineUserFriendList), new AddAgreementDone(add_agreement_done));
                all_agreements.Add(t_agreement.Get_agreement_id, temp_add_agreement);
            }

        }


        public Se_BaseBooleanFunctionResult Create(string p_starter_user_name, string p_invited_user_name)
        {
            bool is_there_user_answer = false;
            int i = 0;
            while (i < 3)
            {
                try
                {
                    is_there_user_answer = is_there_user(p_invited_user_name);
                    i = 0;
                    break;
                }
                catch
                {
                    i++;
                    continue;
                }
            }
            if (i > 2)
            {
                return new Se_BooleanFunctionRejResult("a problem occured. please try again.");
            }
            if (!is_there_user_answer)
            {
                return new Se_BooleanFunctionRejResult("user with username: '" + p_invited_user_name + "' doesnt exist");
            }

            if (p_starter_user_name == p_invited_user_name)
            {
                return new Se_BooleanFunctionRejResult("your request for adding '" + p_invited_user_name + "' was not valid");
            }

            bool are_friends_answer = false;
            i = 0;
            while (i < 3)
            {
                try
                {
                    are_friends_answer = are_friends(p_starter_user_name, p_invited_user_name);
                    i = 0;
                    break;
                }
                catch
                {
                    i++;
                    continue;
                }
            }
            if (i > 2)
            {
                return new Se_BooleanFunctionRejResult("a problem occured. please try again.");
            }
            if (are_friends_answer)
            {
                return new Se_BooleanFunctionRejResult("your request for adding '" + p_invited_user_name + "' was not valid because he/she is in your friends list");
            }

            int random_agreement_id = HelperFunctions.GetGUID();
            AddAgreement temp_add_agreement = new AddAgreement(random_agreement_id, p_starter_user_name, p_invited_user_name, new RemoveAgr(AgreementRemoveItselfRequest)
            , new DB_AddToFriends(add_to_friends), new DB_AreFriends(are_friends), new DB_CreateAddAgreement(create_add_agreement), new DB_RemoveAgreement(remove_agreement)
            , new CreateFormalMessageRequest(Ma_SendFormalMessageToUser), new DB_IsThereUser(is_there_user), new GetUserStatus(Ma_GetUserStatus), new IsLoggedIn(Ma_IsLoggedIn), new StartClientFriendListChangedInformDialog(Ma_StartClientFriendListChangedInformDialog)
            , new ReloadFriendList(Ma_ReloadFriendList), new GetOnlineUserFriendList(Ma_GetOnlineUserFriendList), new AddAgreementDone(add_agreement_done));
            if (all_agreements.ContainsValue(temp_add_agreement))
            {
                return new Se_BooleanFunctionRejResult("This agreement already exist");
            }

            i = 0;
            while (i < 3)
            {
                try
                {
                    create_add_agreement(p_starter_user_name, p_invited_user_name, random_agreement_id);
                    i = 0;
                    break;
                }
                catch
                {
                    i++;
                    continue;
                }
            }
            if (i > 2)
            {
                return new Se_BooleanFunctionRejResult("a problem occured. please try again.");
            }

            all_agreements.Add(random_agreement_id, temp_add_agreement);

            if (is_logged_in(p_invited_user_name))
            {
                start_client_invited_agreement_inform(p_invited_user_name, random_agreement_id);
            }

            return new Se_BooleanFunctionAccResult();
        }

        public override void AgreementRemoveItselfRequest(int p_agreement_id)
        {
            if (all_agreements.ContainsKey(p_agreement_id))
            {
                all_agreements.Remove(p_agreement_id);
            }
        }

        public override void GetAgreementAnswer(string p_user_name, int p_agreement_id, bool p_answer)
        {
            if (all_agreements.ContainsKey(p_agreement_id))
            {
                all_agreements[p_agreement_id].ReceiveAnswer(p_user_name, p_answer);
            }
        }

        public void Ma_StartClientFriendListChangedInformDialog(string p_user_name, List<PersonStatus> p_new_friend_liast_and_status)
        {
            start_client_friend_list_changed_inform_dialog(p_user_name, p_new_friend_liast_and_status);
        }
        public void Ma_ReloadFriendList(string p_user_name)
        {
            reload_friend_list(p_user_name);
        }
        public List<string> Ma_GetOnlineUserFriendList(string p_user_name)
        {
            return get_online_user_friend_list(p_user_name);
        }
        public UserStatus Ma_GetUserStatus(string p_user_name)
        {
            return get_user_status(p_user_name);
        }
    }

    public class AllAgreements
    {

        CreateFormalMessageRequest send_formal_message_to_user;
        DB_IsThereUser is_there_user;
        DB_AddToFriends add_to_friends;
        DB_AreFriends are_friends;
        DB_CreateAddAgreement create_add_agreement;
        DB_RemoveAgreement remove_agreement;
        IsLoggedIn is_logged_in;
        StartClientFriendListChangedInformDialog start_client_friend_list_changed_inform_dialog;
        ReloadFriendList reload_friend_list;
        GetOnlineUserFriendList get_online_user_friend_list;
        GetUserStatus get_user_status;
        AddAgreementDone add_agreement_done;
        Ma_AddAgreementManager ma_add_agreement_manager;
        GetAllAdAgreements get_all_agreements;
        StartClientInvitedAgreementInform start_client_invited_agreement_inform;

        public AllAgreements(ref Dictionary<int, AddAgreement> p_all_add_agreements, CreateFormalMessageRequest p_send_formal_message_to_user, DB_IsThereUser p_is_there_user
            , DB_AddToFriends p_add_to_friends, DB_AreFriends p_are_friends, DB_CreateAddAgreement p_create_add_agreement, DB_RemoveAgreement p_remove_agreement
            , IsLoggedIn p_is_logged_in, StartClientFriendListChangedInformDialog p_start_client_friend_list_changed_inform_dialog, ReloadFriendList p_reload_friend_list
            , GetOnlineUserFriendList p_get_online_user_friend_list, GetUserStatus p_get_user_status, AddAgreementDone p_add_agreement_done, GetAllAdAgreements p_get_all_agreements
            , StartClientInvitedAgreementInform p_start_client_invited_agreement_inform)
        {
            send_formal_message_to_user = p_send_formal_message_to_user;
            is_there_user = p_is_there_user;
            add_to_friends = p_add_to_friends;
            are_friends = p_are_friends;
            create_add_agreement = p_create_add_agreement;
            remove_agreement = p_remove_agreement;
            is_logged_in = p_is_logged_in;
            start_client_friend_list_changed_inform_dialog = p_start_client_friend_list_changed_inform_dialog;
            reload_friend_list = p_reload_friend_list;
            get_online_user_friend_list = p_get_online_user_friend_list;
            get_user_status = p_get_user_status;
            add_agreement_done = p_add_agreement_done;
            get_all_agreements = p_get_all_agreements;
            start_client_invited_agreement_inform = p_start_client_invited_agreement_inform;

            ma_add_agreement_manager = new Ma_AddAgreementManager(ref p_all_add_agreements, new CreateFormalMessageRequest(AllAg_SendFormlMessageToUser)
            , new DB_IsThereUser(AllAg_IsThereUser), new DB_AddToFriends(AllAg_AddToFriends), new DB_AreFriends(AllAg_AreFriends)
            , new DB_CreateAddAgreement(AllAg_CreateAddAgreement), new DB_RemoveAgreement(AllAg_RemoveAgreement), new IsLoggedIn(AllAg_IsLoggedIn)
            , new StartClientFriendListChangedInformDialog(AllAg_StartClientFriendListChangedInformDialog), new ReloadFriendList(AllAg_ReloadFriendList)
            , new GetOnlineUserFriendList(AllAg_GetOnlineUserFriendList), new GetUserStatus(AllAg_GetUserStatus), add_agreement_done
            , new GetAllAdAgreements(AllAg_GetAllAdAgreements), new StartClientInvitedAgreementInform(AllAg_StartClientInvitedAgreementInform));

        }

        public void GetAgreementAnswer(string p_user_name, int p_agreement_id, bool p_answer, TypeOfAgreement p_agreement_type)
        {
            if (p_agreement_type == TypeOfAgreement.Add)
            {
                ma_add_agreement_manager.GetAgreementAnswer(p_user_name, p_agreement_id, p_answer);
            }
        }

        public Se_BaseBooleanFunctionResult CreateAddAgreement(string p_started_user_name, string p_invited_user_name)
        {
            return ma_add_agreement_manager.Create(p_started_user_name, p_invited_user_name);
        }

        public bool AllAg_IsThereUser(string p_user_name)
        {
            return is_there_user(p_user_name);
        }
        public bool AllAg_AreFriends(string p_first_person_user_name, string p_second_person_user_name)
        {
            return are_friends(p_first_person_user_name, p_second_person_user_name);
        }
        public void AllAg_SendFormlMessageToUser(FormalMessage p_message, string p_receiver_user_name)
        {
            send_formal_message_to_user(p_message, p_receiver_user_name);
        }
        public void AllAg_AddToFriends(string p_first_person_user_name, string p_second_person_user_name)
        {
            add_to_friends(p_first_person_user_name, p_second_person_user_name);
        }
        public void AllAg_CreateAddAgreement(string p_starter_user_name, string p_user_to_add_name, int p_agreement_id)
        {
            create_add_agreement(p_starter_user_name, p_user_to_add_name, p_agreement_id);
        }
        public void AllAg_RemoveAgreement(int p_agreement_id)
        {
            remove_agreement(p_agreement_id);
        }
        public bool AllAg_IsLoggedIn(string p_user_name)
        {
            return is_logged_in(p_user_name);
        }
        public void AllAg_StartClientFriendListChangedInformDialog(string p_user_name, List<PersonStatus> p_new_friend_liast_and_status)
        {
            start_client_friend_list_changed_inform_dialog(p_user_name, p_new_friend_liast_and_status);
        }
        public void AllAg_ReloadFriendList(string p_user_name)
        {
            reload_friend_list(p_user_name);
        }
        public List<string> AllAg_GetOnlineUserFriendList(string p_user_name)
        {
            return get_online_user_friend_list(p_user_name);
        }
        public UserStatus AllAg_GetUserStatus(string p_user_name)
        {
            return get_user_status(p_user_name);
        }
        public List<Agreement> AllAg_GetAllAdAgreements()
        {
            return get_all_agreements();
        }
        public void AllAg_StartClientInvitedAgreementInform(string p_user_name, int p_agreement_id)
        {
            start_client_invited_agreement_inform(p_user_name, p_agreement_id);
        }

    }

    public class Agreement
    {
        int agreement_id;
        public int Get_agreement_id
        {
            get { return agreement_id; }
        }

        string inviting_user_name;
        public string Get_inviting_user_name
        {
            get { return inviting_user_name; }
        }

        string invited_user_name;
        public string Get_invited_user_name
        {
            get { return invited_user_name; }
        }

        public Agreement(int p_agreement_id, string p_inviting_user_name, string p_invited_user_name)
        {
            agreement_id = p_agreement_id;
            invited_user_name = p_invited_user_name;
            inviting_user_name = p_inviting_user_name;
        }

    }

}
