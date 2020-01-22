using System.Collections.Generic;
using UnityEngine;

namespace ZeroByterGames.BlockBuilder {
    public class CursorUser
    {
        public string name;
        public enum Type { Normal };
        public Type type = Type.Normal;

        public CursorUser(string name, Type type)
        {
            this.name = name;
            this.type = type;
        }
    }

    public class CursorController : MonoBehaviour
    {
        public static List<CursorUser> Users = new List<CursorUser>();

        /*private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }*/

        public static void AddUser(string name, CursorUser.Type type = CursorUser.Type.Normal)
        {
            CursorUser user = GetUser(name);
            if (user == null)
            {
                Users.Add(new CursorUser(name, type));
            }
            else
            {
                user.type = type;
            }

            SetCursor();
        }

        public static CursorUser GetUser(string name)
        {
            foreach (CursorUser user in Users)
            {
                if (user.name == name) return user;
            }
            return null;
        }

        public static void RemoveUser(string name)
        {
            Users.Remove(GetUser(name));

            SetCursor();
        }

        public static void RemoveAllUsers()
        {
            Users.Clear();
        }

        public static bool AreThereAnyUsers()
        {
            return Users.Count != 0;
        }

        private static void SetCursor()
        {
            if (Users.Count == 0)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                

                Cursor.SetCursor(null, new Vector2(10, 7), CursorMode.Auto);
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                bool alreadySetCursor = false;

                foreach (CursorUser user in Users)
                {
                    switch (user.type)
                    {
                        case CursorUser.Type.Normal:
                            if (!alreadySetCursor) Cursor.SetCursor(null, new Vector2(10, 7), CursorMode.Auto);
                            break;
                    }
                }
            }
        }

        public static void ListAllUsers()
        {
            print("All cursor users:");
            foreach (CursorUser user in Users)
            {
                print(user.name);
            }
        }

        private void OnDestroy()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}