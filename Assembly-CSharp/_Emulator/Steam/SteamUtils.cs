using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Steamworks;

namespace _Emulator
{
    class SteamUtils
    {
        public static string FriendRelationshipToString(EFriendRelationship relationship)
        {
            switch(relationship)
            {
                case EFriendRelationship.k_EFriendRelationshipNone:
                default:
                    return "None";
                case EFriendRelationship.k_EFriendRelationshipBlocked:
                    return "Blocked";
                case EFriendRelationship.k_EFriendRelationshipRequestRecipient:
                    return "Open Request";
                case EFriendRelationship.k_EFriendRelationshipFriend:
                    return "Friend";
                case EFriendRelationship.k_EFriendRelationshipRequestInitiator:
                    return "Sent Request";
                case EFriendRelationship.k_EFriendRelationshipIgnored:
                    return "Ignored";
                case EFriendRelationship.k_EFriendRelationshipIgnoredFriend:
                    return "Ignored Friend";
            }
        }
    }
}
