using UnityEngine;

public class GUITextureHolder : MonoBehaviour
{
	public Texture2D[] channelTab;

	public Texture2D[] currentChannel;

	public Texture2D[] lobbyTab;

	public Texture2D[] briefingTab;

	public Texture2D[] currentChannelBg;

	public Texture2D[] lobbyChatTab;

	public Texture2D[] messengerTab;

	public Texture2D itemUsing;

	public Texture2D itemPreviewing;

	public Texture2D myItemTopBar;

	public Texture2D shopItemTopBar;

	public Texture2D[] itemMain;

	public Texture2D[] itemWeapon;

	public Texture2D[] itemCloth;

	public Texture2D[] itemAccessory;

	public Texture2D myMapTopBar;

	public Texture2D searchMapTopBar;

	public Texture2D rankMapTopBar;

	public Texture2D[] myMap;

	public Texture2D[] mapType;

	public Texture2D[] roomMode;

	public Texture2D[] mapEditMode;

	public Texture2D[] friendTab;

	public Texture2D[] memoTab;

	public Texture2D unreadMemo;

	public Texture2D[] brickTab;

	public Texture2D[] battleTutorLoadings;

	public Texture2D[] teamMatchLoadings;

	public Texture2D[] mapEditorLoadings;

	public Texture2D[] ChannelTab => channelTab;

	public Texture2D[] CurrentChannel => currentChannel;

	public Texture2D[] LobbyTab => lobbyTab;

	public Texture2D[] BriefingTab => briefingTab;

	public Texture2D[] CurrentChannelBg => currentChannelBg;

	public Texture2D[] LobbyChatTab => lobbyChatTab;

	public Texture2D[] MessengerTab => messengerTab;

	public Texture2D ItemUsing => itemUsing;

	public Texture2D ItemPreviewing => itemPreviewing;

	public Texture2D MyItemTopBar => myItemTopBar;

	public Texture2D ShopItemTopBar => shopItemTopBar;

	public Texture2D[] ItemMain => itemMain;

	public Texture2D[] ItemWeapon => itemWeapon;

	public Texture2D[] ItemCloth => itemCloth;

	public Texture2D[] ItemAccessory => itemAccessory;

	public Texture2D MyMapTopBar => myMapTopBar;

	public Texture2D SearchMapTopBar => searchMapTopBar;

	public Texture2D RankMapTopBar => rankMapTopBar;

	public Texture2D[] MyMap => myMap;

	public Texture2D[] MapType => mapType;

	public Texture2D[] RoomMode => roomMode;

	public Texture2D[] MapEditMode => mapEditMode;

	public Texture2D[] FriendTab => friendTab;

	public Texture2D[] MemoTab => memoTab;

	public Texture2D UnreadMemo => unreadMemo;

	public Texture2D[] BrickTab => brickTab;

	public Texture2D Loading
	{
		get
		{
			switch (Application.loadedLevelName)
			{
			case "BattleTutor":
				return battleTutorLoadings[Random.Range(0, battleTutorLoadings.Length)];
			case "MapEditor":
				return mapEditorLoadings[Random.Range(0, mapEditorLoadings.Length)];
			case "TeamMatch":
				return teamMatchLoadings[Random.Range(0, teamMatchLoadings.Length)];
			default:
				return null;
			}
		}
	}
}
