﻿using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace UWPMessengerClient.MSNP
{
    public static class DatabaseAccess
    {
        public async static Task InitializeDatabase()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync("UWPMessengerClient.db", CreationCollisionOption.OpenIfExists);
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "UWPMessengerClient.db");
            using (SqliteConnection db =
               new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                string contactsTableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS Contacts (Primary_Key INTEGER PRIMARY KEY UNIQUE, " +
                    "UserAccount TEXT NULL, " +
                    "Email TEXT NULL, " +
                    "JSONContact TEXT NULL)";
                string messagesTableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS Messages (Primary_Key INTEGER PRIMARY KEY UNIQUE, " +
                    "User TEXT NULL, " +
                    "Principal TEXT NULL, " +
                    "JSONMessage TEXT NULL)";

                SqliteCommand createContactsTable = new SqliteCommand(contactsTableCommand, db);
                SqliteCommand createMessagesTable = new SqliteCommand(messagesTableCommand, db);
                createContactsTable.ExecuteReader();
                createMessagesTable.ExecuteReader();

            }
        }

        public static void UpdateContact(string userEmail, Contact contact)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "UWPMessengerClient.db");
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand updateCommand = new SqliteCommand
                {
                    Connection = db
                };
                string jsonContact = JsonConvert.SerializeObject(contact);
                Contact contactToAdd = JsonConvert.DeserializeObject<Contact>(jsonContact);
                contactToAdd.PresenceStatus = null;
                jsonContact = JsonConvert.SerializeObject(contactToAdd);
                updateCommand.CommandText = "UPDATE Contacts SET JSONContact = @json_contact WHERE Email = @email AND " +
                    "UserAccount = @account";
                updateCommand.Parameters.AddWithValue("@email", contactToAdd.Email);
                updateCommand.Parameters.AddWithValue("@account", userEmail);
                updateCommand.Parameters.AddWithValue("@json_contact", jsonContact);
                updateCommand.ExecuteReader();

                db.Close();
            }
        }

        public static void AddContactToTable(string userEmail, Contact contact)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "UWPMessengerClient.db");
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand insertCommand = new SqliteCommand
                {
                    Connection = db
                };
                string jsonContact = JsonConvert.SerializeObject(contact);
                // Use parameterized query to prevent SQL injection attacks
                insertCommand.CommandText = "INSERT INTO Contacts VALUES (NULL, @account, @email, @json_contact)";
                insertCommand.Parameters.AddWithValue("@account", userEmail);
                insertCommand.Parameters.AddWithValue("@email", contact.Email);
                insertCommand.Parameters.AddWithValue("@json_contact", jsonContact);
                insertCommand.ExecuteReader();

                db.Close();
            }
        }

        public static void DeleteContactFromTable(string userEmail, Contact contact)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "UWPMessengerClient.db");
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand deleteCommand = new SqliteCommand
                {
                    Connection = db
                };
                deleteCommand.CommandText = "DELETE FROM Contacts WHERE UserAccount = @account AND Email = @contact_email";
                deleteCommand.Parameters.AddWithValue("@account", userEmail);
                deleteCommand.Parameters.AddWithValue("@contact_email", contact.Email);
                deleteCommand.ExecuteReader();

                db.Close();
            }
        }

        public static List<string> GetUserContacts(string userEmail)
        {
            List<string> entries = new List<string>();

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "UWPMessengerClient.db");
            using (SqliteConnection db =
               new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand selectCommand = new SqliteCommand
                    ("SELECT JSONContact from Contacts WHERE UserAccount = @user_account", db);
                selectCommand.Parameters.AddWithValue("@user_account", userEmail);
                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    entries.Add(query.GetString(0));
                }

                db.Close();
            }
            return entries;
        }

        public static void AddMessageToTable(string userEmail, string principalEmail, Message message)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "UWPMessengerClient.db");
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand insertCommand = new SqliteCommand
                {
                    Connection = db
                };
                string jsonMessage = JsonConvert.SerializeObject(message);
                // Use parameterized query to prevent SQL injection attacks
                insertCommand.CommandText = "INSERT INTO Messages VALUES (NULL, @user, @principal, @json_message)";
                insertCommand.Parameters.AddWithValue("@user", userEmail);
                insertCommand.Parameters.AddWithValue("@principal", principalEmail);
                insertCommand.Parameters.AddWithValue("@json_message", jsonMessage);
                insertCommand.ExecuteReader();

                db.Close();
            }
        }

        public static List<string> ReturnMessagesFromSenderAndReceiver(string userEmail, string principalEmail)
        {
            List<string> entries = new List<string>();

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "UWPMessengerClient.db");
            using (SqliteConnection db =
               new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand selectCommand = new SqliteCommand
                    ("SELECT JSONMessage from Messages WHERE User = @user AND Principal = @principal", db);
                selectCommand.Parameters.AddWithValue("@user", userEmail);
                selectCommand.Parameters.AddWithValue("@principal", principalEmail);
                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    entries.Add(query.GetString(0));
                }

                db.Close();
            }
            return entries;
        }
    }
}
