
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonChatTypes;

namespace ServerTcpChat.Classes
{
    class DB_Controller
    {
        DB_Model model;
        public DB_Controller()
        {
            model = new DB_Model();
        }

        public void AddNewUser(string p_user_name, string p_password)
        {
            model.AddNewUser(p_user_name, p_password);
        }

        public void InsertFriendshipRelation(string p_first_person_user_name, string p_second_person_user_name)
        {
            model.InsertFriendshipRelation(p_first_person_user_name, p_second_person_user_name);
        }

        public bool IsThereUser(string p_user_name)
        {
            return model.IsThereUser(p_user_name);
        }

        public bool IsThereUserPass(string p_user_name, string p_passworrd)
        {
            return model.IsThereUserPass(p_user_name, p_passworrd);
        }

        public bool AreFriends(string p_first_person_user_name, string p_second_person_user_name)
        {
            return model.AreFriends(p_first_person_user_name, p_second_person_user_name);
        }

        public void AddOfflineMessage(int p_message_id, string p_sender_user_name, string p_receiver_user_name, string p_message_text)
        {
            model.AddOfflineMessage(p_message_id, p_sender_user_name, p_receiver_user_name, p_message_text);
        }

        public List<OfflineMessage> LoadUserOfflineMessages(string p_user_name)
        {
            return model.LoadUserOflineMessages(p_user_name);
        }

        public void CreateAddAgreement(int p_agreement_id, string p_starter_user_name, string p_invited_user_name)
        {
            model.CreateAddAgreement(p_agreement_id, p_starter_user_name, p_invited_user_name);
        }

        public void RemoveAgreement(int p_agreement_id)
        {
            model.RemoveAgreement(p_agreement_id);
        }

        public List<AgreementInvitationInfo> GetUserAgreementInvitation(string p_user_name)
        {
            return model.GetUserAgreementInvitation(p_user_name);
        }

        public AgreementInvitationInfo GetAUserAgreementInvitation(string p_user_name, int p_agreement_id)
        {
            return model.GetAUserAgreementInvitation(p_user_name, p_agreement_id);
        }

        public void AddToFriends(string p_first_person_user_name, string p_second_person_user_name)
        {
            model.AddToFriends(p_first_person_user_name, p_second_person_user_name);
        }

        public List<string> GetUserFriendsList(string p_user_name)
        {
            return model.GetUserFriendsList(p_user_name);
        }

        public void RemoveUserOfflineMessages(string p_user_name, List<int> p_message_ids)
        {
            model.RemoveUserOfflineMessages(p_user_name, p_message_ids);
        }

        public bool IsThereAgreement(int p_agreement_id)
        {
            return model.IsThereAgreement(p_agreement_id);
        }

        public void AddAgreementDone(int p_agreement_id)
        {
            model.AddAgreementDone(p_agreement_id);
        }

        public List<Agreement> GetAllAddAgreements()
        {
            return model.GetAllAdAgreements();
        }
    }
}
