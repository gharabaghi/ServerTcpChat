using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using CommonChatTypes;

namespace ServerTcpChat.Classes
{
    class DB_Model
    {
        SqlConnection db_connection;

        public DB_Model()
        {

            string init_connection_string = Properties.Settings.Default["TcpServer_DBConnectionString"].ToString();
            SqlConnectionStringBuilder connection_string_builder = new SqlConnectionStringBuilder(init_connection_string);
            string app_exe_file = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string app_exe_path = System.IO.Path.GetDirectoryName(app_exe_file);
            string db_file = app_exe_path + "\\DataBase\\TcpServer_DB.mdf";
            connection_string_builder.AttachDBFilename = db_file;
            connection_string_builder.InitialCatalog = "TcpServer_DB";
            db_connection = new SqlConnection(connection_string_builder.ConnectionString);
            db_connection.Open();
        }

        public void AddNewUser(string p_user_name, string p_password)
        {
            string command_string = "insert into users_data values ('" + p_user_name + "' , '" + p_password + "');";
            using (SqlCommand command = new SqlCommand(command_string, db_connection))
            {
                if (command.Connection.State == ConnectionState.Closed)
                    db_connection.Open();
                command.ExecuteNonQuery();


            }
            return;
        }

        public void InsertFriendshipRelation(string p_first_person_user_name, string p_second_person_user_name)
        {
            string command_string = "insert into friendship_relations values ('" + p_first_person_user_name + "' , '" + p_second_person_user_name + "');";
            using (SqlCommand command = new SqlCommand(command_string, db_connection))
            {
                if (command.Connection.State == ConnectionState.Closed)
                    db_connection.Open();
                command.ExecuteNonQuery();
            }
            return;
        }

        public bool IsThereUser(string p_user_name)
        {
            string command_string = "select user_name from users_data where user_name = '" + p_user_name + "';"; ;
            using (SqlCommand command = new SqlCommand(command_string, db_connection))
            {
                if (db_connection.State == ConnectionState.Closed)
                    db_connection.Open();
                SqlDataReader data_reader = command.ExecuteReader();
                DataTable data_table = new DataTable();
                data_table.Load(data_reader);
                if (data_table.Rows.Count > 0)
                    return true;
            }
            return false;
        }

        public bool IsThereUserPass(string p_user_name, string p_passworrd) 
        {
            string command_string = "select user_name from users_data where user_name = '" + p_user_name + "' AND user_password = '" + p_passworrd + "';";
            using (SqlCommand command = new SqlCommand(command_string, db_connection))
            {
                if (db_connection.State == ConnectionState.Closed)
                    db_connection.Open();
                SqlDataReader data_reader = command.ExecuteReader();
                DataTable data_table = new DataTable();
                data_table.Load(data_reader);
                if (data_table.Rows.Count > 0)
                    return true;
            }
            return false;
        }

        public bool AreFriends(string p_first_person_user_name, string p_second_person_user_name)
        {
            string command_string = "select * from friendship_relations where (first_person_user_name = '" + p_first_person_user_name
             + "' AND second_person_user_name = '" + p_second_person_user_name + "') OR  (first_person_user_name = '" + p_second_person_user_name
             + "' AND second_person_user_name = '" + p_first_person_user_name + "');";
            using (SqlCommand command = new SqlCommand(command_string, db_connection))
            {
                if (db_connection.State == ConnectionState.Closed)
                    db_connection.Open();
                SqlDataReader data_reader = command.ExecuteReader();
                DataTable data_table = new DataTable();
                data_table.Load(data_reader);
                if (data_table.Rows.Count > 0)
                    return true;
            }
            return false;
        }

        public void AddOfflineMessage(int p_message_id, string p_sender_user_name, string p_receiver_user_name, string p_message_text)
        {
            string command_string = "insert into offline_messages (message_id, sender_user_name, receiver_user_name, message_text) values ('" + p_message_id.ToString() + "' , '" + p_sender_user_name
                + "' ,'" + p_receiver_user_name + "' ,'" + p_message_text + "') ;";
            using (SqlCommand command = new SqlCommand(command_string, db_connection))
            {
                if (db_connection.State == ConnectionState.Closed)
                    db_connection.Open();
                command.ExecuteNonQuery();
            }
            return;
        }

        public List<OfflineMessage> LoadUserOflineMessages(string p_user_name)
        {
            List<OfflineMessage> user_offline_messages = new List<OfflineMessage>();
            DataTable data_table = new DataTable();
            string command_string = "Select * from offline_messages where receiver_user_name = '" + p_user_name + "'";
            using (SqlCommand command = new SqlCommand(command_string, db_connection))
            {
                if (db_connection.State == ConnectionState.Closed)
                    db_connection.Open();
                SqlDataReader data_reader = command.ExecuteReader();
                data_table.Load(data_reader);
            }
            foreach (DataRow off_row in data_table.Rows)
            {
                user_offline_messages.Add(new OfflineMessage(Convert.ToInt32(off_row["message_id"]), off_row["sender_user_name"].ToString(), off_row["message_text"].ToString()));
            }
            return user_offline_messages;

        }

        public void CreateAddAgreement(int p_agreement_id, string p_starter_user_name, string p_invited_user_name)
        {
            int? type_num = null;

            string get_agreement_type_num_command_string = "select * from agreement_type";
            using (SqlCommand get_agreement_type_num_command = new SqlCommand(get_agreement_type_num_command_string, db_connection))
            {
                if (get_agreement_type_num_command.Connection.State == ConnectionState.Closed)
                    get_agreement_type_num_command.Connection.Open();
                DataTable agreement_type_nums_table = new DataTable();
                agreement_type_nums_table.Load(get_agreement_type_num_command.ExecuteReader());
                foreach (DataRow type_row in agreement_type_nums_table.Rows)
                {
                    if (type_row["Type"].ToString().ToLower().Trim() == "add")
                        type_num = Convert.ToInt32(type_row["type_num"].ToString());
                }
            }
            if (type_num == null)
            {
                throw new KeyNotFoundException("Add agreement type is not in the agreement_type tble");
            }
            string insert_agreement_command_string = "insert into agreements values(" + p_agreement_id.ToString() + ", '"
                + p_starter_user_name + "', " + type_num.ToString() + ");";

            int? result_num = null;
            string get_agreement_result_num_command_string = "select * from agreement_result";
            using (SqlCommand get_agreement_result_num_command = new SqlCommand(get_agreement_result_num_command_string, db_connection))
            {
                if (get_agreement_result_num_command.Connection.State == ConnectionState.Closed)
                    get_agreement_result_num_command.Connection.Open();
                DataTable agreement_result_nums_table = new DataTable();
                agreement_result_nums_table.Load(get_agreement_result_num_command.ExecuteReader());
                foreach (DataRow type_row in agreement_result_nums_table.Rows)
                {
                    if (type_row["result"].ToString().ToLower().Trim() == "unanswered")
                        result_num = Convert.ToInt32(type_row["result_num"].ToString());
                }
            }
            if (result_num == null)
            {
                throw new KeyNotFoundException("unanswered agreement result is not in the agreement_result table");
            }
            string insert_invitations_commands_string = "insert into agreement_invitations values(" + p_agreement_id + ", "
                + "'" + p_invited_user_name + "', " + result_num.ToString() + ");";

            SqlTransaction insert_agreement_transaction = null;

            SqlCommand insert_agreement_command = new SqlCommand(insert_agreement_command_string, db_connection);
            SqlCommand insert_agreement_invitation_command = new SqlCommand(insert_invitations_commands_string, db_connection);
            try
            {
                if (db_connection.State == ConnectionState.Closed)
                    db_connection.Open();

                insert_agreement_transaction = db_connection.BeginTransaction();

                insert_agreement_command.Transaction = insert_agreement_transaction;
                insert_agreement_invitation_command.Transaction = insert_agreement_transaction;

                insert_agreement_command.ExecuteNonQuery();
                insert_agreement_invitation_command.ExecuteNonQuery();

                insert_agreement_transaction.Commit();
            }
            catch (Exception ex)
            {
                insert_agreement_transaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }

        }

        public void RemoveAgreement(int p_agreement_id)
        {
            string remove_agreement_command_string = "delete from agreements where id = " + p_agreement_id + ";";
            string remove_agreemane_invitations_command_string = "delete from agreement_invitations where id = " + p_agreement_id + ";";

            SqlTransaction remove_transaction = null;

            SqlCommand remove_agreement_command = new SqlCommand(remove_agreement_command_string, db_connection);
            SqlCommand remove_agreement_invitation_command = new SqlCommand(remove_agreemane_invitations_command_string, db_connection);

            try
            {
                if (db_connection.State == ConnectionState.Closed)
                    db_connection.Open();

                remove_transaction = db_connection.BeginTransaction();

                remove_agreement_command.Transaction = remove_transaction;
                remove_agreement_invitation_command.Transaction = remove_transaction;

                remove_agreement_command.ExecuteNonQuery();
                remove_agreement_invitation_command.ExecuteNonQuery();

                remove_transaction.Commit();
            }
            catch (Exception ex)
            {
                remove_transaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public List<AgreementInvitationInfo> GetUserAgreementInvitation(string p_user_name)
        {
            List<AgreementInvitationInfo> user_agreement_invitations = new List<AgreementInvitationInfo>();

            string command_string = "select * from agreements, agreement_invitations, agreement_result, agreement_type where " +
            "agreements.id = agreement_invitations.id AND agreement_invitations.result = agreement_result.result_num AND " +
            "agreements.Type = agreement_type.Type_Num AND invited_user_name = '" + p_user_name + "' AND agreement_result.result= " +
            "'unanswered';";
            using (SqlCommand command = new SqlCommand(command_string, db_connection))
            {
                if (db_connection.State == ConnectionState.Closed)
                    db_connection.Open();
                DataTable user_agreement_invitations_table = new DataTable();
                SqlDataReader t = command.ExecuteReader();
                user_agreement_invitations_table.Load(t);

                foreach (DataRow t_row in user_agreement_invitations_table.Rows)
                {
                    TypeOfAgreement temp_agreement_type = TypeOfAgreement.Add;
                    if (t_row["Type"].ToString().ToLower().Trim() == "add")
                        temp_agreement_type = TypeOfAgreement.Add;
                    user_agreement_invitations.Add(new AgreementInvitationInfo(t_row["starter_user_name"].ToString(),
                        t_row["type_text"].ToString(), Convert.ToInt32(t_row["ID"].ToString()), temp_agreement_type));
                }
            }
            return user_agreement_invitations;

        }

        public AgreementInvitationInfo GetAUserAgreementInvitation(string p_user_name, int p_agreement_id)
        {
            AgreementInvitationInfo result_agreement_invitation = null;

            string command_string = "select * from agreements, agreement_invitations, agreement_result, agreement_type where " +
            "agreements.id = " + p_agreement_id +" AND agreements.id = agreement_invitations.id AND agreement_invitations.result = agreement_result.result_num AND " +
            "agreements.Type = agreement_type.Type_Num AND invited_user_name = '" + p_user_name + "' AND agreement_result.result= " +
            "'unanswered';";

            using (SqlCommand command = new SqlCommand(command_string, db_connection))
            {
                if (db_connection.State == ConnectionState.Closed)
                    db_connection.Open();
                DataTable user_agreement_invitations_table = new DataTable();
                SqlDataReader t = command.ExecuteReader();
                user_agreement_invitations_table.Load(t);

                TypeOfAgreement temp_agreement_type = TypeOfAgreement.Add;
                if (user_agreement_invitations_table.Rows.Count > 0)
                {
                    temp_agreement_type = TypeOfAgreement.Add;
                    DataRow t_row = user_agreement_invitations_table.Rows[0];
                    result_agreement_invitation = new AgreementInvitationInfo(t_row["starter_user_name"].ToString(),
                        t_row["type_text"].ToString(), Convert.ToInt32(t_row["ID"].ToString()), temp_agreement_type);
                }
            }
                return result_agreement_invitation;
        }

        public void AddToFriends(string p_first_person_user_name, string p_second_person_user_name)
        {
            string command_string = "insert into friendship_relations values ('" + p_first_person_user_name + "' , '" + p_second_person_user_name + "';";
            using (SqlCommand command = new SqlCommand(command_string, db_connection))
            {
                if (db_connection.State == ConnectionState.Closed)
                    db_connection.Open();
                command.ExecuteNonQuery();
            }
            return;
        }

        public List<string> GetUserFriendsList(string p_user_name)
        {
            List<string> result = new List<string>();

            string command_string = "select * from Friendship_relations where first_person_user_name = '" + p_user_name
                + "' OR second_person_user_name = '" + p_user_name + "' ;";
            using (SqlCommand command = new SqlCommand(command_string, db_connection))
            {
                if (db_connection.State == ConnectionState.Closed)
                    db_connection.Open();
                SqlDataReader data_reader = command.ExecuteReader();
                DataTable data_table = new DataTable();
                data_table.Load(data_reader);
                foreach (DataRow t_row in data_table.Rows)
                {
                    if (t_row["first_person_user_name"].ToString() == p_user_name || t_row["second_person_user_name"].ToString() == p_user_name)
                    {
                        if (t_row["first_person_user_name"].ToString() != p_user_name)
                        {
                            result.Add(t_row["first_person_user_name"].ToString());
                        }
                        else if (t_row["second_person_user_name"].ToString() != p_user_name)
                        {
                            result.Add(t_row["second_person_user_name"].ToString());
                        }
                    }
                }
            }
            return result;
        }

        public void RemoveUserOfflineMessages(string p_user_name, List<int> p_message_ids)
        {
            string command_string = "Delete from offline_messages where receiver_user_name = '" + p_user_name + "'"
                + "and message_id = ";
            foreach (int p_id in p_message_ids)
            {
                command_string = command_string + p_id.ToString() + " ;";
                using (SqlCommand command = new SqlCommand(command_string, db_connection))
                {
                    if (db_connection.State == ConnectionState.Closed)
                        db_connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }

        public bool IsThereAgreement(int p_agreement_id)
        {
            string command_string = "select starter_user_name from agreements, agreement_invitations where"
                + " agreements.id = agreement_invitations.id and agreement.id = " + p_agreement_id + " ;";
            using (SqlCommand command = new SqlCommand(command_string, db_connection))
            {
                if (db_connection.State == ConnectionState.Closed)
                    db_connection.Open();
                SqlDataReader data_reader = command.ExecuteReader();
                DataTable data_table = new DataTable();
                data_table.Load(data_reader);
                if (data_table.Rows.Count > 0)
                    return true;
            }
            return false;
        }

        public void AddAgreementDone(int p_agreement_id)
        {
            string starter_user_name = "";
            string invited_user_name = "";

            string find_starter_user_name_command_string = "select starter_user_name from agreements where id = " + p_agreement_id.ToString() + "; ";
            using (SqlCommand command = new SqlCommand(find_starter_user_name_command_string, db_connection))
            {
                if (db_connection.State == ConnectionState.Closed)
                    db_connection.Open();
                SqlDataReader data_reader = command.ExecuteReader();
                DataTable data_table = new DataTable();
                data_table.Load(data_reader);
                starter_user_name = data_table.Rows[0]["starter_user_name"].ToString();
            }

            string find_invited_user_name_command_string = "select * from agreement_invitations where id = " + p_agreement_id.ToString() + " ;";
            using (SqlCommand command = new SqlCommand(find_invited_user_name_command_string, db_connection))
            {
                if (db_connection.State == ConnectionState.Closed)
                    db_connection.Open();
                SqlDataReader data_reader = command.ExecuteReader();
                DataTable data_table = new DataTable();
                data_table.Load(data_reader);
                invited_user_name = data_table.Rows[0]["invited_user_name"].ToString();
            }


            string add_command_string = "insert into friendship_relations values ('" + starter_user_name + "' , '"
                + invited_user_name + "') ;";
            SqlCommand add_command = new SqlCommand(add_command_string, db_connection);

            string remove_agreement_command_string = "delete from agreements where id = " + p_agreement_id + " ;";
            SqlCommand remove_agreement_command = new SqlCommand(remove_agreement_command_string, db_connection);

            string remove_agreemane_invitations_command_string = "delete from agreement_invitations where id = " + p_agreement_id + " ;";
            SqlCommand remove_agreemane_invitations_command = new SqlCommand(remove_agreemane_invitations_command_string, db_connection);

            SqlTransaction transaction = null;

            try
            {
                if (db_connection.State == ConnectionState.Closed)
                    db_connection.Open();

                transaction = db_connection.BeginTransaction();

                add_command.Transaction = transaction;
                remove_agreement_command.Transaction = transaction;
                remove_agreemane_invitations_command.Transaction = transaction;

                add_command.ExecuteNonQuery();
                remove_agreement_command.ExecuteNonQuery();
                remove_agreemane_invitations_command.ExecuteNonQuery();

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public List<Agreement> GetAllAdAgreements()
        {
            string select_all_agreements_command_string = "select agreements.id, invited_user_name, starter_user_name from agreements, agreement_invitations, agreement_type " +
                "where agreements.type = agreement_type.type_num and agreement_type.type = 'add' and agreements.id = agreement_invitations.id; ";

            List<Agreement> result_agreements = new List<Agreement>();

            using (SqlCommand select_all_agreement_command = new SqlCommand(select_all_agreements_command_string, db_connection))
            {
                if (db_connection.State == ConnectionState.Closed)
                    db_connection.Open();

                SqlDataReader data_reader = select_all_agreement_command.ExecuteReader();
                DataTable data_table = new DataTable();
                data_table.Load(data_reader);

                foreach (DataRow t_row in data_table.Rows)
                {
                    int t_agreement_id = Convert.ToInt32(t_row["ID"].ToString());
                    string t_invited_user_name = t_row["invited_user_name"].ToString();
                    string t_inviting_user_name = t_row["starter_user_name"].ToString();
                    Agreement t_agreement = new Agreement(t_agreement_id, t_inviting_user_name, t_invited_user_name);
                    result_agreements.Add(t_agreement);
                }
            }
            return result_agreements;

        }

    }

}