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

        public static string PersonaStateToString(EPersonaState state)
        {
            switch (state)
            { 
                case EPersonaState.k_EPersonaStateOffline:
                    return "Offline";
                case EPersonaState.k_EPersonaStateOnline:
                    return "Online";
                case EPersonaState.k_EPersonaStateBusy:
                    return "Busy";
                case EPersonaState.k_EPersonaStateAway:
                    return "Away";
                case EPersonaState.k_EPersonaStateSnooze:
                    return "Snooze";
                case EPersonaState.k_EPersonaStateLookingToTrade:
                    return "Looking to trade";
                case EPersonaState.k_EPersonaStateLookingToPlay:
                    return "Looking to play";
                case EPersonaState.k_EPersonaStateInvisible:
                    return "Invisible";
                default:
                    return "Unknown";
            }
        }
    }
}
