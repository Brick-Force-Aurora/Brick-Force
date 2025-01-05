using _Emulator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;
using wlic;
public class SockTcp
{
	private const int REQ_SUCCESS = 0;

	private const int FAIL_TO_FIND_ACCOUNT = -1;

	private const int INVALID_PASSWORD = -2;

	private const int DUPLICATE_LOGIN = -3;

	private const int INVALID_VERSION = -4;

	private const int SYSTEM_DEAD = -111;

	private const int GM_BLOCK_USER = -115;

	private const int NO_MORE_ROOM = -1;

	private const int QUICK_JOIN_FAIL = -1;

	private const int NOT_READY_YET = -1;

	private const int MASTER_ONLY = -2;

	private const int LOAD_MAP_FAIL = -3;

	private const int CLAN_MATCH_MIXED = -4;

	private const int CLAN_MATCH_SAME_CLAN = -5;

	private const int CLAN_MATCH_TOO_SMALL = -6;

	private const int CLAN_MATCH_NOT_SAME_NO = -7;

	private const int CAN_NOT_PLAY_WITH_NO_ENEMY = -8;

	private const int AT_LEAST_ONE_FOE_READY = -9;

	private const int BLOCKED_MAP = -10;

	private const int CANT_BREAK_INTO = -1;

	private const int NOT_PLAYING = -2;

	private const sbyte WIN_END = 1;

	private const sbyte DRAW_END = 0;

	private const sbyte LOSE_END = -1;

	private const int INVITE_SUCCESS = 0;

	private const int INVITE_FAIL_NO_ROOM = -1;

	private const int INVITE_FAIL_INVITEE_BUSY = -2;

	private const int INVITE_FAIL_CANT_REACH = -3;

	private const int INVITE_FAIL_ROOM_FULL = -4;

	private const int INVITE_FAIL_PENDING = -6;

	private const int INVITE_FAIL_NO_BREAK_INTO = -7;

	private const int INVITE_FAIL_CLAN_MATCH = -8;

	private const int INVITE_FAIL_DENY_ALL = -9;

	private const int INVITE_FAIL_BAN = -10;

	private const int INVITE_FAIL_FRIEND_ONLY = -11;

	private const int INVITE_FAIL_KICKED_ONCE = -12;

	private const int FOLLOW_SUCCESS = 0;

	private const int FOLLOW_FAIL_ALREADY_ROOM = -1;

	private const int FOLLOW_FAIL_FOLLOWEE_NO_ROOM = -2;

	private const int FOLLOW_FAIL_CANT_REACH = -3;

	private const int FOLLOW_FAIL_ROOM_FULL = -4;

	private const int FOLLOW_FAIL_LOCKED = -5;

	private const int FOLLOW_FAIL_PENDING = -6;

	private const int FOLLOW_FAIL_NO_BREAK_INTO = -7;

	private const int FOLLOW_FAIL_CLAN_MATCH = -8;

	private const int FOLLOW_FAIL_BAN = -10;

	private const int FOLLOW_FAIL_KICKED_ONCE = -12;

	private const int FOLLOW_FAIL_GM = -13;

	private const ushort CS_LOGIN_REQ = 1;

	private const ushort CS_LOGIN_ACK = 2;

	private const ushort CS_HEARTBEAT_REQ = 3;

	private const ushort CS_ROOM_LIST_REQ = 4;

	private const ushort CS_ADD_ROOM_ACK = 5;

	private const ushort CS_DEL_ROOM_ACK = 6;

	private const ushort CS_CREATE_ROOM_REQ = 7;

	private const ushort CS_CREATE_ROOM_ACK = 8;

	private const ushort CS_QUICK_JOIN_REQ = 9;

	private const ushort CS_ENTER_ACK = 10;

	private const ushort CS_LEAVE_ACK = 11;

	private const ushort CS_QUICK_JOIN_ACK = 12;

	private const ushort CS_ADD_BRICK_REQ = 13;

	private const ushort CS_ADD_BRICK_ACK = 14;

	private const ushort CS_DEL_BRICK_REQ = 15;

	private const ushort CS_DEL_BRICK_ACK = 16;

	private const ushort CS_EDIT_BRICK_FAIL_ACK = 17;

	private const ushort CS_PALETTE_ACK = 18;

	private const ushort CS_SAVE_PALETTE_REQ = 19;

	private const ushort CS_CACHE_BRICK_REQ = 20;

	private const ushort CS_CACHE_BRICK_ACK = 21;

	private const ushort CS_CACHE_BRICK_DONE_ACK = 22;

	private const ushort CS_LEAVE_REQ = 23;

	private const ushort CS_CHAT_REQ = 24;

	private const ushort CS_CHAT_ACK = 25;

	private const ushort CS_WHISPER_REQ = 26;

	private const ushort CS_PLAYER_INFO_ACK = 27;

	private const ushort CS_JOIN_REQ = 28;

	private const ushort CS_JOIN_ACK = 29;

	private const ushort CS_UPDATE_ROOM_ACK = 30;

	private const ushort CS_MASTER_ACK = 31;

	private const ushort CS_RESUME_ROOM_REQ = 32;

	private const ushort CS_MORPH_BRICK_REQ = 33;

	private const ushort CS_ITEM_ACK = 34;

	private const ushort CS_EQUIP_REQ = 35;

	private const ushort CS_EQUIP_ACK = 36;

	private const ushort CS_UNEQUIP_REQ = 37;

	private const ushort CS_UNEQUIP_ACK = 38;

	private const ushort CS_SAVE_REQ = 39;

	private const ushort CS_SAVE_ACK = 40;

	private const int SAVE_OK = 0;

	private const int SAVE_ERROR = -1;

	private const int SAVE_NO_OWNERSHIP = -2;

	private const ushort CS_USERMAP_ACK = 41;

	private const ushort CS_LOAD_COMPLETE_REQ = 42;

	private const ushort CS_LOAD_COMPLETE_ACK = 43;

	private const ushort CS_KILL_LOG_REQ = 44;

	private const ushort CS_KILL_LOG_ACK = 45;

	private const ushort CS_SLOT_INFO_ACK = 46;

	private const ushort CS_SET_STATUS_REQ = 47;

	private const ushort CS_SET_STATUS_ACK = 48;

	private const ushort CS_START_REQ = 49;

	private const ushort CS_START_ACK = 50;

	private const ushort CS_REGISTER_REQ = 51;

	private const ushort CS_REGISTER_ACK = 52;

	private const ushort CS_COPYRIGHT_ACK = 53;

	private const ushort CS_CHANGE_USERMAP_ALIAS_REQ = 54;

	private const ushort CS_CHANGE_USERMAP_ALIAS_ACK = 55;

	private const ushort CS_NEW_MAP_LIST_REQ = 56;

	private const ushort CS_NEW_MAP_LIST_ACK = 57;

	private const ushort CS_RANK_MAP_LIST_REQ = 58;

	private const ushort CS_RANK_MAP_LIST_ACK = 59;

	private const ushort CS_STAR_DUST_REQ = 60;

	private const ushort CS_REG_MAP_INFO_ACK = 61;

	private const ushort CS_RESPAWN_TICKET_REQ = 63;

	private const ushort CS_RESPAWN_TICKET_ACK = 64;

	private const ushort CS_TIMER_REQ = 65;

	private const ushort CS_TIMER_ACK = 66;

	private const ushort CS_TEAM_SCORE_ACK = 67;

	private const ushort CS_DEATH_COUNT_ACK = 68;

	private const ushort CS_KILL_COUNT_ACK = 69;

	private const ushort CS_TEAM_MATCH_END_ACK = 70;

	private const ushort CS_MATCH_COUNTDOWN_REQ = 71;

	private const ushort CS_MATCH_COUNTDOWN_ACK = 72;

	private const ushort CS_BREAK_INTO_REQ = 73;

	private const ushort CS_BREAK_INTO_ACK = 74;

	private const ushort CS_TEAM_SCORE_REQ = 75;

	private const ushort CS_DESTROY_BRICK_REQ = 76;

	private const ushort CS_DESTROY_BRICK_ACK = 77;

	private const ushort CS_DESTROYED_BRICK_ACK = 78;

	private const ushort CS_LOAD_START_ACK = 79;

	private const ushort CS_TEAM_CHANGE_REQ = 80;

	private const ushort CS_TEAM_CHANGE_ACK = 81;

	private const ushort CS_SERIAL_BONUS_REQ = 82;

	private const ushort CS_RANDEZVOUS_POINT_REQ = 83;

	private const ushort CS_SEED_ACK = 84;

	private const ushort CS_SLOT_LOCK_REQ = 85;

	private const ushort CS_SLOT_LOCK_ACK = 86;

	private const ushort CS_TEAM_CHANGE_FAIL_ACK = 87;

	private const ushort CS_KICK_REQ = 88;

	private const ushort CS_KICK_ACK = 89;

	private const ushort CS_MAKE_ROOM_NULL_REQ = 90;

	private const ushort CS_ROOM_CONFIG_REQ = 91;

	private const ushort CS_ROOM_CONFIG_ACK = 92;

	private const ushort CS_TEAM_CHAT_REQ = 93;

	private const ushort CS_CLAN_CHAT_REQ = 94;

	private const ushort CS_RADIO_MSG_REQ = 95;

	private const ushort CS_RADIO_MSG_ACK = 96;

	private const ushort CS_WHISPER_FAIL_ACK = 97;

	private const ushort CS_MY_MAP_LIST_REQ = 98;

	private const ushort CS_MY_MAP_LIST_ACK = 99;

	private const ushort CS_DOWNLOAD_THUMBNAIL_REQ = 100;

	private const ushort CS_DOWNLOAD_THUMBNAIL_ACK = 101;

	private const ushort CS_ASSET_ACK = 102;

	private const ushort CS_ADD_FRIEND_REQ = 103;

	private const ushort CS_ADD_FRIEND_ACK = 104;

	private const ushort CS_ADD_BAN_REQ = 105;

	private const ushort CS_ADD_BAN_ACK = 106;

	private const ushort CS_DEL_FRIEND_REQ = 107;

	private const ushort CS_DEL_FRIEND_ACK = 108;

	private const ushort CS_DEL_BAN_REQ = 109;

	private const ushort CS_DEL_BAN_ACK = 110;

	private const ushort CS_ADD_FRIEND_BY_NICKNAME_REQ = 111;

	private const ushort CS_ADD_BAN_BY_NICKNAME_REQ = 112;

	private const ushort CS_ADD_FRIEND_FAIL_ACK = 113;

	private const ushort CS_DEL_FRIEND_FAIL_ACK = 114;

	private const ushort CS_ADD_BAN_FAIL_ACK = 115;

	private const ushort CS_DEL_BAN_FAIL_ACK = 116;

	private const ushort CS_ADD_FRIEND_BY_NICKNAME_FAIL_ACK = 117;

	private const ushort CS_ADD_BAN_BY_NICKNAME_FAIL_ACK = 118;

	private const ushort CS_SVC_ENTER_ACK = 119;

	private const ushort CS_SVC_LEAVE_ACK = 120;

	private const ushort CS_BUY_ITEM_REQ = 121;

	private const ushort CS_BUY_ITEM_ACK = 122;

	private const ushort CS_CHECK_TERM_ITEM_REQ = 123;

	private const ushort CS_TERM_ITEM_EXPIRED_ACK = 124;

	private const ushort CS_MEMO_ACK = 125;

	private const ushort CS_SEND_MEMO_REQ = 126;

	private const ushort CS_SEND_MEMO_ACK = 127;

	private const ushort CS_PRESENT_ITEM_REQ = 128;

	private const ushort CS_PRESENT_ITEM_ACK = 129;

	private const ushort CS_DEL_MEMO_REQ = 130;

	private const ushort CS_DEL_MEMO_ACK = 131;

	private const ushort CS_RCV_PRESENT_REQ = 132;

	private const ushort CS_RCV_PRESENT_ACK = 133;

	private const ushort CS_READ_MEMO_REQ = 134;

	private const ushort CS_P2P_COMPLETE_REQ = 135;

	private const ushort CS_P2P_COMPLETE_ACK = 136;

	private const ushort CS_RESULT_DONE_REQ = 137;

	private const ushort CS_RESULT_DONE_ACK = 138;

	private const ushort CS_ROUND_ROBIN_ACK = 139;

	private const ushort CS_CHANNEL_LIST_REQ = 140;

	private const ushort CS_CHANNEL_ACK = 141;

	private const ushort CS_CHANNEL_END_ACK = 142;

	private const ushort CS_ROAMOUT_REQ = 143;

	private const ushort CS_ROAMOUT_ACK = 144;

	private const ushort CS_ROAMIN_REQ = 145;

	private const ushort CS_ROAMIN_ACK = 146;

	private const ushort CS_CUR_CHANNEL_ACK = 147;

	private const ushort CS_PLAYER_INIT_INFO_ACK = 148;

	private const ushort CS_DUPLICATE_REPORT_ACK = 149;

	private const ushort CS_WHATSUP_FELLA_REQ = 150;

	private const ushort CS_LEVELUP_EVENT_REQ = 151;

	private const ushort CS_LEVELUP_EVENT_ACK = 152;

	private const ushort CS_RENDEZVOUS_POINT_ACK = 153;

	private const ushort CS_VERSION_CHECK_REQ = 154;

	private const ushort CS_PATCH_ACK = 155;

	private const ushort CS_PATCH_END_ACK = 156;

	private const ushort CS_LOGOUT_REQ = 157;

	private const ushort CS_GET_CANNON_REQ = 158;

	private const ushort CS_GET_CANNON_ACK = 159;

	private const ushort CS_EMPTY_CANNON_REQ = 160;

	private const ushort CS_EMPTY_CANNON_ACK = 161;

	private const ushort CS_XTRAP_PACKET = 162;

	private const ushort CS_GOT_CANNON_ACK = 163;

	private const ushort CS_DOWNLOAD_THUMBNAIL_END_ACK = 164;

	private const ushort CS_SERVICE_FAIL_ACK = 165;

	private const ushort CS_WAITING_QUEUING_ACK = 166;

	private const ushort CS_UPDATE_SCRIPT_REQ = 167;

	private const ushort CS_UPDATE_SCRIPT_ACK = 168;

	private const ushort CS_UPDATE_SCRIPT_FAIL_ACK = 169;

	private const ushort CS_TUTORIAL_COMPLETE_REQ = 170;

	private const ushort CS_TUTORIAL_COMPLETE_ACK = 171;

	private const ushort CS_APS_WARNING_ACK = 172;

	private const ushort CS_DOWNLOADED_MAP_ACK = 173;

	private const ushort CS_DOWNLOAD_MAP_REQ = 174;

	private const ushort CS_DOWNLOAD_MAP_ACK = 175;

	private const ushort CS_DEL_DOWNLOAD_MAP_REQ = 176;

	private const ushort CS_DEL_DOWNLOAD_MAP_ACK = 177;

	private const ushort CS_INDIVIDUAL_SCORE_REQ = 178;

	private const ushort CS_INDIVIDUAL_SCORE_ACK = 179;

	private const ushort CS_INDIVIDUAL_MATCH_END_ACK = 180;

	private const ushort CS_CORE_HP_ACK = 181;

	private const ushort CS_MISSION_END_ACK = 182;

	private const ushort CS_PLAYER_DETAIL_REQ = 183;

	private const ushort CS_PLAYER_DETAIL_ACK = 184;

	private const ushort CS_ASSIST_COUNT_ACK = 185;

	private const ushort CS_GM_SAYS_REQ = 186;

	private const ushort CS_CHECK_CLAN_NAME_AVAILABILITY_REQ = 187;

	private const ushort CS_CHECK_CLAN_NAME_AVAILABILITY_ACK = 188;

	private const ushort CS_CREATE_CLAN_REQ = 189;

	private const ushort CS_CREATE_CLAN_ACK = 190;

	private const ushort CS_DESTROY_CLAN_REQ = 191;

	private const ushort CS_DESTROY_CLAN_ACK = 192;

	private const ushort CS_SEND_CLAN_INVITATION_REQ = 193;

	private const ushort CS_SEND_CLAN_INVITATION_ACK = 194;

	private const ushort CS_ANSWER_CLAN_INVITATION_REQ = 195;

	private const ushort CS_ANSWER_CLAN_INVITATION_ACK = 196;

	private const ushort CS_RESET_MY_CLAN_ACK = 197;

	private const ushort CS_LEAVE_CLAN_REQ = 198;

	private const ushort CS_LEAVE_CLAN_ACK = 199;

	private const ushort CS_SELECT_CLAN_MEMBER_REQ = 200;

	private const ushort CS_CLAN_MEMBER_ACK = 201;

	private const ushort CS_CLAN_MEMBER_END_ACK = 202;

	private const ushort CS_SELECT_CLAN_INTRO_REQ = 203;

	private const ushort CS_SELECT_CLAN_INTRO_ACK = 204;

	private const ushort CS_ROUND_END_ACK = 205;

	private const ushort CS_SELECT_CLAN_APPLICANT_REQ = 206;

	private const ushort CS_CLAN_APPLICANT_ACK = 207;

	private const ushort CS_CLAN_APPLICANT_END_ACK = 208;

	private const ushort CS_CLAN_KICK_REQ = 209;

	private const ushort CS_CLAN_KICK_ACK = 210;

	private const ushort CS_UP_CLAN_MEMBER_REQ = 211;

	private const ushort CS_UP_CLAN_MEMBER_ACK = 212;

	private const ushort CS_DOWN_CLAN_MEMBER_REQ = 213;

	private const ushort CS_DOWN_CLAN_MEMBER_ACK = 214;

	private const ushort CS_TRANSFER_MASTER_REQ = 215;

	private const ushort CS_TRANSFER_MASTER_ACK = 216;

	private const ushort CS_ACCEPT_APPLICANT_REQ = 217;

	private const ushort CS_ACCEPT_APPLICANT_ACK = 218;

	private const ushort CS_OPEN_RANDOM_BOX_REQ = 219;

	private const ushort CS_OPEN_RANDOM_BOX_ACK = 220;

	private const ushort CS_APPLY_CLAN_REQ = 221;

	private const ushort CS_APPLY_CLAN_ACK = 222;

	private const ushort CS_CHANGE_CLAN_INTRO_REQ = 223;

	private const ushort CS_CHANGE_CLAN_INTRO_ACK = 224;

	private const ushort CS_CHANGE_CLAN_NOTICE_REQ = 225;

	private const ushort CS_CHANGE_CLAN_NOTICE_ACK = 226;

	private const ushort CS_CLAN_DETAIL_REQ = 227;

	private const ushort CS_CLAN_DETAIL_ACK = 228;

	private const ushort CS_ADD_CLANEE_ACK = 229;

	private const ushort CS_CLAN_INFO_CHANGE_ACK = 230;

	private const ushort CS_CLAN_NEW_MEMBER_ACK = 231;

	private const ushort CS_CLAN_DEL_MEMBER_ACK = 232;

	private const ushort CS_CLAN_MEMBER_REQ = 233;

	private const ushort CS_CHANGE_CLAN_MARK_REQ = 234;

	private const ushort CS_CHANGE_CLAN_MARK_ACK = 235;

	private const ushort CS_CLAN_MARK_CHANGED_ACK = 236;

	private const ushort CS_CREATE_SQUAD_REQ = 237;

	private const ushort CS_CREATE_SQUAD_ACK = 238;

	private const ushort CS_JOIN_SQUAD_REQ = 239;

	private const ushort CS_JOIN_SQUAD_ACK = 240;

	private const ushort CS_LEAVE_SQUAD_REQ = 241;

	private const ushort CS_ENTER_SQUAD_ACK = 242;

	private const ushort CS_LEAVE_SQUAD_ACK = 247;

	private const ushort CS_ADD_SQUAD_ACK = 248;

	private const ushort CS_DEL_SQUAD_ACK = 249;

	private const ushort CS_UPDATE_SQUAD_ACK = 250;

	private const ushort CS_MATCH_TEAM_START_REQ = 251;

	private const ushort CS_MATCH_TEAM_START_ACK = 252;

	private const ushort CS_ENTER_SQUADING_REQ = 253;

	private const ushort CS_ENTER_SQUADING_ACK = 254;

	private const ushort CS_LEAVE_SQUADING_REQ = 255;

	private const ushort CS_LEAVE_SQUADING_ACK = 256;

	private const ushort CS_KICK_SQUAD_REQ = 257;

	private const ushort CS_KICK_SQUAD_ACK = 258;

	private const ushort CS_CHG_SQUAD_OPTION_REQ = 259;

	private const ushort CS_CHG_SQUAD_OPTION_ACK = 260;

	private const ushort CS_CLAN_MATCH_HALF_TIME_ACK = 261;

	private const ushort CS_GET_BACK2SPAWNER_REQ = 262;

	private const ushort CS_GET_BACK2SPAWNER_ACK = 263;

	private const ushort CS_MATCH_RESTART_COUNT_REQ = 264;

	private const ushort CS_MATCH_RESTART_COUNT_ACK = 265;

	private const ushort CS_MATCH_RESTARTED_REQ = 266;

	private const ushort CS_MATCH_RESTARTED_ACK = 267;

	private const ushort CS_CLAN_MATCH_RECORD_LIST_REQ = 268;

	private const ushort CS_CLAN_MATCH_RECORD_LIST_ACK = 269;

	private const ushort CS_CLAN_MATCH_PLAYER_LIST_REQ = 270;

	private const ushort CS_CLAN_MATCH_PLAYER_LIST_ACK = 271;

	private const ushort CS_MATCH_TEAM_CANCEL_REQ = 272;

	private const ushort CS_MATCH_TEAM_CANCEL_ACK = 273;

	private const ushort CS_MATCH_TEAM_SUCCESS_ACK = 274;

	private const ushort CS_CLAN_MATCH_TEAM_GETBACK_REQ = 275;

	private const ushort CS_CLAN_MATCH_TEAM_GETBACK_ACK = 276;

	private const ushort CS_MATCH_TEAM_CANCELED_ACK = 277;

	private const ushort CS_UPDATE_SQUAD_RECORD_REQ = 278;

	private const ushort CS_BM_INSTALL_BOMB_REQ = 279;

	private const ushort CS_BM_INSTALL_BOMB_ACK = 280;

	private const ushort CS_BM_UNINSTALL_BOMB_REQ = 281;

	private const ushort CS_BM_UNINSTALL_BOMB_ACK = 282;

	private const ushort CS_BM_BLAST_REQ = 283;

	private const ushort CS_BM_BLAST_ACK = 284;

	private const ushort CS_CTF_PICK_FLAG_REQ = 285;

	private const ushort CS_CTF_PICK_FLAG_ACK = 286;

	private const ushort CS_CTF_CAPTURE_FLAG_REQ = 287;

	private const ushort CS_CTF_CAPTURE_FLAG_ACK = 288;

	private const ushort CS_CTF_DROP_FLAG_REQ = 289;

	private const ushort CS_CTF_DROP_FLAG_ACK = 290;

	private const ushort CS_BLAST_MODE_END_ACK = 291;

	private const ushort CS_CAPTURE_THE_FLAG_END_ACK = 292;

	private const ushort CS_BLAST_MODE_SCORE_REQ = 293;

	private const ushort CS_BLAST_MODE_SCORE_ACK = 294;

	private const ushort CS_CTF_SCORE_REQ = 295;

	private const ushort CS_CTF_SCORE_ACK = 296;

	private const ushort CS_CLAN_LIST_REQ = 297;

	private const ushort CS_CLAN_LIST_ACK = 298;

	private const ushort CS_MISSION_COUNT_ACK = 299;

	private const ushort CS_ROUND_SCORE_ACK = 300;

	private const ushort CS_BM_STATUS_ACK = 301;

	private const ushort CS_CTF_STATUS_ACK = 302;

	private const ushort CS_ME_EDITOR_LIST_ACK = 303;

	private const ushort CS_ME_CHG_EDITOR_REQ = 304;

	private const ushort CS_ME_CHG_EDITOR_ACK = 305;

	private const ushort CS_MAKE_SQUAD_NULL_REQ = 306;

	private const ushort CS_INIT_TERM_ITEM_REQ = 307;

	private const ushort CS_INIT_TERM_ITEM_ACK = 308;

	private const ushort CS_REBUY_ITEM_REQ = 309;

	private const ushort CS_REBUY_ITEM_ACK = 310;

	private const ushort CS_ERASE_DELETED_ITEM_REQ = 311;

	private const ushort CS_ERASE_DELETED_ITEM_ACK = 312;

	private const ushort CS_ME_PREMIUM_BRICKS_ACK = 313;

	private const ushort CS_WEAPON_LEVEL_ACK = 314;

	private const ushort CS_BACK2BREIFING_ACK = 315;

	private const ushort CS_DISCOMPOSE_ITEM_REQ = 316;

	private const ushort CS_DISCOMPOSE_ITEM_ACK = 317;

	private const ushort CS_SHOP_ACK = 318;

	private const ushort CS_STAR_LEVEL_ACK = 319;

	private const ushort CS_RENDEZVOUS_INFO_ACK = 320;

	private const ushort CS_USERMAP_END_ACK = 321;

	private const ushort CS_SHOP_END_ACK = 322;

	private const ushort CS_WEAPON_MODIFIER_ACK = 323;

	private const ushort CS_BND_SCORE_REQ = 324;

	private const ushort CS_LINE_BRICK_REQ = 325;

	private const ushort CS_LINE_BRICK_ACK = 326;

	private const ushort CS_REPLACE_BRICK_REQ = 327;

	private const ushort CS_REPLACE_BRICK_ACK = 328;

	private const ushort CS_USE_SHOOTER_CONSUMABLE_REQ = 329;

	private const ushort CS_LINE_BRICK_FAIL_ACK = 330;

	private const ushort CS_REPLACE_BRICK_FAIL_ACK = 331;

	private const ushort CS_SHOOTER_TOOL_ACK = 332;

	private const ushort CS_SET_SHOOTER_TOOL_REQ = 333;

	private const ushort CS_CLEAR_SHOOTER_TOOLS_REQ = 334;

	private const ushort CS_REG_MAP_INFO_REQ = 337;

	private const ushort CS_BND_MODE_END_ACK = 338;

	private const ushort CS_BND_SCORE_ACK = 339;

	private const ushort CS_ME_REG_BRICK_ACK = 340;

	private const ushort CS_ME_REG_BRICK_END_ACK = 341;

	private const ushort CS_CORE_HP_REQ = 342;

	private const ushort CS_DELETED_MAP_ACK = 343;

	private const ushort CS_BND_SHIFT_PHASE_ACK = 344;

	private const ushort CS_STACK_POINT_REQ = 345;

	private const ushort CS_NOTICE_ACK = 346;

	private const ushort CS_AD_ACK = 347;

	private const ushort CS_CHG_COUNTRY_FILTER_REQ = 348;

	private const ushort CS_BND_SHIFT_PHASE_REQ = 349;

	private const ushort CS_WEAPON_DURABILITY_ACK = 350;

	private const ushort CS_REPAIR_WEAPON_REQ = 351;

	private const ushort CS_REPAIR_WEAPON_ACK = 352;

	private const ushort CS_UPGRADE_ITEM_REQ = 353;

	private const ushort CS_UPGRADE_ITEM_ACK = 354;

	private const ushort CS_ITEM_PIMP_ACK = 355;

	private const ushort CS_PIMP_MODIFIER_ACK = 356;

	private const ushort CS_MERGE_ITEM_REQ = 357;

	private const ushort CS_MERGE_ITEM_ACK = 358;

	private const ushort CS_BUNDLE_ACK = 359;

	private const ushort CS_UNPACK_BUNDLE_REQ = 360;

	private const ushort CS_UNPACK_BUNDLE_ACK = 361;

	private const ushort CS_PROMO_LIST_ACK = 362;

	private const ushort CS_BOUGHTONCE_LIST_ACK = 363;

	private const ushort CS_I_AGREE_TOS_REQ = 364;

	private const ushort CS_I_AGREE_TOS_ACK = 365;

	private const ushort CS_CTF_FLAG_RETURN_REQ = 366;

	private const ushort CS_CTF_FLAG_RETURN_ACK = 367;

	private const ushort CS_WEAPON_HELD_RATIO_REQ = 368;

	private const ushort CS_TC_OPEN_REQ = 369;

	private const ushort CS_TC_OPEN_ACK = 370;

	private const ushort CS_TC_CLOSE_REQ = 371;

	private const ushort CS_TC_ENTER_REQ = 372;

	private const ushort CS_TC_ENTER_ACK = 373;

	private const ushort CS_TC_OPEN_PRIZE_TAG_REQ = 374;

	private const ushort CS_TC_CHEST_ACK = 375;

	private const ushort CS_TC_OPEN_PRIZE_TAG_ACK = 376;

	private const ushort CS_TC_TOOKOFF_ACK = 377;

	private const ushort CS_TC_UPDATE_CHEST_ACK = 378;

	private const ushort CS_TC_RECEIVE_PRIZE_REQ = 379;

	private const ushort CS_TC_RECEIVE_PRIZE_ACK = 380;

	private const ushort CS_MISSION_ACK = 381;

	private const ushort CS_MISSION_WAIT_ACK = 382;

	private const ushort CS_ACCEPT_DAILY_MISSION_REQ = 383;

	private const ushort CS_ACCEPT_DAILY_MISSION_ACK = 384;

	private const ushort CS_GIVEUP_DAILY_MISSION_REQ = 385;

	private const ushort CS_COMPLETE_DAILY_MISSION_REQ = 386;

	private const ushort CS_COMPLETE_DAILY_MISSION_ACK = 387;

	private const ushort CS_PROGRESS_DAILY_MISSION_ACK = 388;

	private const ushort CS_DELEGATE_MASTER_REQ = 389;

	private const ushort CS_STARTING_ACK = 390;

	private const ushort CS_TREASURE_ACK = 391;

	private const ushort CS_TC_LEAVE_REQ = 392;

	private const ushort CS_AUTO_LOGIN_REQ = 393;

	private const ushort CS_AUTO_LOGIN_ACK = 394;

	private const ushort CS_HASH_CHECK_REQ = 395;

	private const ushort CS_HASH_CHECK_ERROR_ACK = 396;

	private const ushort CS_CHARGE_PICKNWIN_COIN_REQ = 397;

	private const ushort CS_CHARGE_PICKNWIN_COIN_ACK = 398;

	private const ushort CS_INFLICTED_DAMAGE_REQ = 399;

	private const ushort CS_INSTALL_GADGET_REQ = 400;

	private const ushort CS_GADGET_ACK = 401;

	private const ushort CS_GADGET_ACTION_REQ = 402;

	private const ushort CS_GADGET_ACTION_ACK = 403;

	private const ushort CS_GADGET_REMOVE_ACK = 404;

	private const ushort CS_RESET_USER_MAP_SLOTS_REQ = 405;

	private const ushort CS_RESET_USER_MAP_SLOTS_ACK = 406;

	private const ushort CS_INC_EXTRA_SLOTS_REQ = 407;

	private const ushort CS_INC_EXTRA_SLOTS_ACK = 408;

	private const ushort CS_AUTO_LOGIN_TO_RUNUP_REQ = 409;

	private const ushort CS_AUTO_LOGIN_TO_RUNUP_ACK = 410;

	private const ushort CS_CREATE_CHARACTER_REQ = 411;

	private const ushort CS_CREATE_CHARACTER_ACK = 412;

	private const ushort CS_WEAPON_CHANGE_REQ = 414;

	private const ushort CS_WEAPON_CHANGE_ACK = 415;

	private const ushort CS_PLAYER_WEAPON_CHANGE_ACK = 416;

	private const ushort CS_PLAYER_OPT_ACK = 417;

	private const ushort CS_WEAPON_SLOT_ACK = 418;

	private const ushort CS_SET_WEAPON_SLOT_REQ = 419;

	private const ushort CS_CLEAR_WEAPON_SLOTS_REQ = 420;

	private const ushort CS_GENERIC_BUNDLE_ACK = 421;

	private const ushort CS_CUSTOM_STRING_ACK = 422;

	private const ushort CS_MAP_EVAL_REQ = 423;

	private const ushort CS_MAP_EVAL_ACK = 424;

	private const ushort CS_MY_DOWNLOAD_MAP_REQ = 425;

	private const ushort CS_MY_DOWNLOAD_MAP_ACK = 426;

	private const ushort CS_MY_REGISTER_MAP_REQ = 427;

	private const ushort CS_MY_REGISTER_MAP_ACK = 428;

	private const ushort CS_USER_MAP_REQ = 429;

	private const ushort CS_USER_MAP_ACK = 430;

	private const ushort CS_ALL_MAP_REQ = 431;

	private const ushort CS_ALL_MAP_ACK = 432;

	private const ushort CS_WEEKLY_CHART_REQ = 433;

	private const ushort CS_WEEKLY_CHART_ACK = 434;

	private const ushort CS_MAP_HONOR_REQ = 435;

	private const ushort CS_MAP_HONOR_ACK = 436;

	private const ushort CS_MAP_DETAIL_REQ = 437;

	private const ushort CS_MAP_DETAIL_ACK = 438;

	private const ushort CS_MORE_COMMENT_REQ = 439;

	private const ushort CS_MORE_COMMENT_ACK = 440;

	private const ushort CS_CHG_MAP_INTRO_REQ = 441;

	private const ushort CS_CHG_MAP_INTRO_ACK = 442;

	private const ushort CS_CHG_MAP_DOWNLOAD_FREE_REQ = 443;

	private const ushort CS_CHG_MAP_DOWNLOAD_FREE_ACK = 444;

	private const ushort CS_MAP_DAY_REQ = 445;

	private const ushort CS_MAP_DAY_ACK = 446;

	private const ushort CS_OPEN_DOOR_REQ = 447;

	private const ushort CS_CLOSE_DOOR_REQ = 448;

	private const ushort CS_CHG_DOOR_STATUS_ACK = 449;

	private const ushort CS_OPEN_DOOR_ACK = 450;

	private const ushort CS_BUILDER_SLOT_ACK = 451;

	private const ushort CS_WEAPON_MODIFIER_EX_ACK = 452;

	private const ushort CS_ROOM_CONFIG_FAIL_ACK = 453;

	private const ushort CS_SLOT_CHANGE_ACK = 454;

	private const ushort CS_MUTE_REQ = 455;

	private const ushort CS_MUTE_ACK = 456;

	private const ushort CS_YOU_ARE_MUTE_ACK = 457;

	private const ushort CS_AUTO_LOGIN_TO_NETMARBLE_REQ = 458;

	private const ushort CS_AUTO_LOGIN_TO_NETMARBLE_ACK = 459;

	private const ushort CS_SAVE_PLAYER_COMMON_OPT_REQ = 460;

	private const ushort CS_USERMAP_LIST_ACK = 461;

	private const ushort CS_SHOOTER_TOOL_LIST_ACK = 462;

	private const ushort CS_WEAPON_SLOT_LIST_ACK = 463;

	private const ushort CS_ITEM_LIST_ACK = 464;

	private const ushort CS_DOWNLOADED_MAP_LIST_ACK = 465;

	private const ushort CS_DELETED_MAP_LIST_ACK = 466;

	private const ushort CS_SVC_ENTER_LIST_ACK = 467;

	private const ushort CS_ROOM_LIST_ACK = 468;

	private const ushort CS_ROOM_REQ = 469;

	private const ushort CS_ROOM_ACK = 470;

	private const ushort CS_CHARGE_FORCE_POINT_REQ = 471;

	private const ushort CS_CHARGE_FORCE_POINT_ACK = 472;

	private const ushort CS_XP_ACK = 473;

	private const ushort CS_BUNGEE_SCORE_REQ = 474;

	private const ushort CS_BUNGEE_SCORE_ACK = 475;

	private const ushort CS_BUNGEE_MODE_END_ACK = 476;

	private const ushort CS_CUR_CHANNEL_SPECIFIC_INFO_ACK = 477;

	private const ushort CS_CHANNEL_PLAYER_LIST_REQ = 478;

	private const ushort CS_BATCH_DEL_BRICK_REQ = 479;

	private const ushort CS_BATCH_DEL_BRICK_ACK = 480;

	private const ushort CS_INVITE_REQ = 481;

	private const ushort CS_INVITE_ACK = 482;

	private const ushort CS_INVITED_ACK = 483;

	private const ushort CS_FOLLOWING_REQ = 484;

	private const ushort CS_FOLLOWING_ACK = 485;

	private const ushort CS_NMPLAYAUTH_STATE_NOTI_ACK = 486;

	private const ushort CS_CPTR_ACK = 487;

	private const ushort CS_CPTR_REWARD_ACK = 488;

	private const ushort CS_LEVELUP_REWARD_ACK = 489;

	private const ushort CS_MISSION_REWARD_ACK = 490;

	private const ushort CS_ITEM_PROPERTY_ACK = 491;

	private const ushort CS_PREMIUM_ITEM_ACK = 492;

	private const ushort CS_NETCAFE_ITEM_ACK = 493;

	private const ushort CS_START_KICKOUT_VOTE_REQ = 494;

	private const ushort CS_START_KICKOUT_VOTE_ACK = 495;

	private const ushort CS_KICKOUT_VOTE_REQ = 496;

	private const ushort CS_KICKOUT_VOTE_ACK = 497;

	private const ushort CS_KICKOUT_VOTE_STATUS_ACK = 498;

	private const ushort CS_KICKOUT_VOTE_END_ACK = 499;

	private const ushort CS_CUSTOM_GAME_CONFIG_ACK = 500;

	private const ushort CS_USE_CHANGE_NICKNAME_REQ = 501;

	private const ushort CS_USE_CHANGE_NICKNAME_ACK = 502;

	private const ushort CS_NICKNAME_CHANGE_ACK = 503;

	private const ushort CS_LOGIN_TO_AXESO5_REQ = 504;

	private const ushort CS_LOGIN_TO_AXESO5_ACK = 505;

	private const ushort CS_BND_STATUS_ACK = 506;

	private const ushort CS_BUNGEE_STATUS_ACK = 507;

	private const ushort CS_MISSION_POINT_REQ = 508;

	private const ushort CS_MISSION_POINT_ACK = 509;

	private const ushort CS_ACCUSE_PLAYER_REQ = 510;

	private const ushort CS_ACCUSE_PLAYER_ACK = 511;

	private const ushort CS_ACCUSE_MAP_REQ = 512;

	private const ushort CS_ACCUSE_MAP_ACK = 513;

	private const ushort CS_RESET_BATTLE_RECORD_REQ = 514;

	private const ushort CS_RESET_BATTLE_RECORD_ACK = 515;

	private const ushort CS_GM_COMMAND_USAGE_LOG_REQ = 516;

	private const ushort CS_RANDOM_INVITE_REQ = 517;

	private const ushort CS_RANDOM_INVITE_ACK = 518;

	private const ushort CS_ESCAPE_SCORE_REQ = 519;

	private const ushort CS_ESCAPE_SCORE_ACK = 520;

	private const ushort CS_ESCAPE_MODE_END_ACK = 521;

	private const ushort CS_ESCAPE_STATUS_ACK = 522;

	private const ushort CS_ESCAPE_GOAL_REQ = 523;

	private const ushort CS_ESCAPE_GOAL_ACK = 524;

	private const ushort CS_ESCAPE_ACTIVE_PLAYER_REQ = 525;

	private const ushort CS_DROP_ITEM_REQ = 526;

	private const ushort CS_DROP_ITEM_ACK = 527;

	private const ushort CS_PICKUP_DROPPED_ITEM_REQ = 528;

	private const ushort CS_PICKUP_DROPPED_ITEM_ACK = 529;

	private const ushort CS_DEL_DROP_ITEM_ACK = 530;

	private const ushort CS_DROPITEM_QUICK_JOIN_USER_ACK = 531;

	private const ushort CS_STAR_RATE_ACK = 532;

	private const ushort CS_PICKUP_DROPPED_ITEM_FAIL_ACK = 533;

	private const ushort CS_MASTER_KICKING_REQ = 534;

	private const ushort CS_MASTER_KICKING_ACK = 535;

	private const ushort CS_ZOMBIE_MODE_SCORE_ACK = 536;

	private const ushort CS_ZOMBIE_END_ACK = 537;

	private const ushort CS_ZOMBIE_INFECTION_REQ = 538;

	private const ushort CS_ZOMBIE_INFECTION_ACK = 539;

	private const ushort CS_ZOMBIE_INFECT_REQ = 540;

	private const ushort CS_ZOMBIE_INFECT_ACK = 541;

	private const ushort CS_SELECT_WANTED_ACK = 542;

	private const ushort CS_KNOW_WANTED_ACK = 543;

	private const ushort CS_DESELECT_WANTED_ACK = 544;

	private const ushort CS_ZOMBIE_MODE_SCORE_REQ = 545;

	private const ushort CS_ZOMBIE_BREAK_INTO_ACK = 546;

	private const ushort CS_ZOMBIE_STATUS_REQ = 547;

	private const ushort CS_ZOMBIE_STATUS_ACK = 548;

	private const ushort CS_ZOMBIE_OBSERVER_REQ = 549;

	private const ushort CS_ADD_AMMO_ACK = 550;

	private const ushort CS_GET_TRAIN_REQ = 551;

	private const ushort CS_GET_TRAIN_ACK = 552;

	private const ushort CS_EMPTY_TRAIN_REQ = 553;

	private const ushort CS_EMPTY_TRAIN_ACK = 554;

	private const ushort CS_CLAN_APPLY_LIST_REQ = 555;

	private const ushort CS_CLAN_APPLY_LIST_ACK = 556;

	private const ushort CS_CLAN_CANCEL_APPLICATION_REQ = 557;

	private const ushort CS_CLAN_CANCEL_APPLICATION_ACK = 558;

	private const ushort CS_CLAN_APPLICANT_COUNT_ACK = 559;

	private const ushort CS_CLAN_MATCHING_LIST_REQ = 560;

	private const ushort CS_CLAN_MATCHING_LIST_ACK = 561;

	private const ushort CS_CLAN_NEED_CREATE_POINT_REQ = 562;

	private const ushort CS_CLAN_NEED_CREATE_POINT_ACK = 563;

	private const ushort CS_CLAN_CHANGE_ROOM_NAME_ACK = 564;

	private const ushort CS_CLAN_GET_OUT_SQUAD_ACK = 565;

	private const int KICKOUT_VOTE_SUCCESS = 0;

	private const int KICKOUT_VOTE_FAIL_DENIED_EVER = -1;

	private const int KICKOUT_VOTE_FAIL_SELF = -2;

	private const int KICKOUT_VOTE_FAIL_ONGOING_ALREADY = -3;

	private const int KICKOUT_VOTE_FAIL_VOTEE_OUTOF_ROOOM = -4;

	private const int KICKOUT_VOTE_FAIL_TOO_SMALL_VOTERS = -5;

	private const int KICKOUT_VOTE_FAIL_STARTED_EVER = -6;

	private const int KICKOUT_VOTE_FAIL_VOTER_OUTOF_ROOOM = -7;

	private const int KICKOUT_VOTE_FAIL_UNKNOWN = -64;

	private const int KICKOUT_VOTE_ACK_SUCCESS = 0;

	private const int KICKOUT_VOTE_ACK_FAIL_RETRY_VOTE = -2;

	private const int KICKOUT_VOTE_ACK_FAIL_NO_VOTE_TICKET = -3;

	private const int KICKOUT_VOTE_ACK_FAIL_NO_PROGRESS_VOTE = -4;

	private const int KICKOUT_VOTE_ACK_FAIL_UNKNOWN = -64;

	private const int ACCUSE_PLAYER_ACK_SUCCESS = 0;

	private const int ACCUSE_PLAYER_ACK_FAIL_NO_USER = -1;

	private const int ACCUSE_PLAYER_ACK_FAIL_SAME_ACCUSE = -2;

	private const int ACCUSE_PLAYER_ACK_FAIL_COUNT_OVER = -3;

	private const int ACCUSE_PLAYER_ACK_FAIL_SELF_OR_GM = -4;

	private const int ACCUSE_PLAYER_ACK_FAIL_SERVER_ERROR = -5;

	private const int RANDOM_INVITE_SUCCESS = 0;

	private const int RANDOM_INVITE_NO_PLAYER = -1;

	private const int RANDOM_INVITE_MASTER_ONLY = -2;

	private const int RANDOM_INVITE_NOT_SUPPORT = -3;

	public Socket _sock;

	private Queue _writeQueue;

	private Queue _readQueue;

	private Msg4Recv _recv;

	private float _onceASecond;

	public bool _heartbeat;

	public bool _waitingAck;

	private long holdrand;

	private string privateIpAddress = string.Empty;

	private string macAddress = string.Empty;

	private byte recvKey = byte.MaxValue;

	private byte[] sendKeys;

	private int sendKeyIndex;

	public bool WaitingAck => _waitingAck;

	public bool IsBroken => _sock == null;

	public int RecvKey => recvKey;

	private void srandMine(byte seed)
	{
		holdrand = (int)seed;
	}

	private int randMine()
	{
		return (int)(((holdrand = holdrand * 214013 + 2531011) >> 16) & 0x7FFF);
	}

	private void Init()
	{
		if (_writeQueue == null)
		{
			_writeQueue = new Queue();
		}
		if (_readQueue == null)
		{
			_readQueue = new Queue();
		}
		if (_recv == null)
		{
			_recv = new Msg4Recv();
		}
		recvKey = byte.MaxValue;
		sendKeyIndex = 0;
		sendKeys = null;
		_waitingAck = false;
	}

	private void GetMacAddress()
	{
		try
		{
			macAddress = string.Empty;
			NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			NetworkInterface[] array = allNetworkInterfaces;
			int num = 0;
			PhysicalAddress physicalAddress;
			while (true)
			{
				if (num >= array.Length)
				{
					return;
				}
				NetworkInterface networkInterface = array[num];
				physicalAddress = networkInterface.GetPhysicalAddress();
				if (physicalAddress != null && !physicalAddress.ToString().Equals(string.Empty))
				{
					break;
				}
				num++;
			}
			macAddress += physicalAddress.ToString();
		}
		catch (Exception ex)
		{
			Debug.LogError("Error " + ex.Message.ToString() + " FetchMacAddress ");
		}
	}

	public bool Open(string ip, int port)
	{
		Close();
		Property props = BuildOption.Instance.Props;
		if (props.isWebPlayer)
		{
			Security.PrefetchSocketPolicy(ip, 843);
		}
		bool result = false;
		try
		{
			Init();
			_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			_sock.Connect(ip, port);
			if (_sock.Connected)
			{
				_sock.BeginReceive(_recv.Buffer, _recv.Io, _recv.Buffer.Length - _recv.Io, SocketFlags.None, ReceiveCallback, null);
				result = true;
				privateIpAddress = ((IPEndPoint)_sock.LocalEndPoint).Address.ToString();
				GetMacAddress();
				return result;
			}
			Debug.LogError("_sock.Connect Failed");
			return result;
		}
		catch (Exception ex)
		{
			Debug.LogError("Error, " + ex.Message.ToString());
			Close();
			return result;
		}
	}

	public void Close()
	{
		_heartbeat = false;
		if (_sock != null)
		{
			_sock.Close();
			_sock = null;
		}
		if (_writeQueue != null)
		{
			_writeQueue.Clear();
			_writeQueue = null;
		}
		if (_readQueue != null)
		{
			_readQueue.Clear();
			_readQueue = null;
		}
		_onceASecond = 0f;
		_heartbeat = false;
		recvKey = byte.MaxValue;
		sendKeyIndex = 0;
		sendKeys = null;
		_recv = null;
		_waitingAck = false;
	}

	public void Update()
	{
		_waitingAck = false;
		if (_readQueue != null)
		{
			if (_heartbeat)
			{
				_onceASecond += Time.deltaTime;
				if (_onceASecond > 1f)
				{
					_onceASecond = 0f;
					SendCS_HEARTBEAT_REQ(MyInfoManager.Instance.UseGmFunction());
				}
			}
			lock (this)
			{
				while (_readQueue != null && _readQueue.Count > 0)
				{
					Msg2Handle msg2Handle = (Msg2Handle)_readQueue.Peek();
                    Debug.LogWarning("Message with ID: " + msg2Handle._id);

                    if (!ClientExtension.instance.HandleMessage(msg2Handle))
					switch (msg2Handle._id)
					{
					case 2:
						HandleCS_LOGIN_ACK(msg2Handle._msg);
						break;
					case 5:
						HandleCS_ADD_ROOM_ACK(msg2Handle._msg);
						break;
					case 8:
						HandleCS_CREATE_ROOM_ACK(msg2Handle._msg);
						break;
					case 12:
						HandleCS_QUICK_JOIN_ACK(msg2Handle._msg);
						break;
					case 10:
						HandleCS_ENTER_ACK(msg2Handle._msg);
						break;
					case 11:
						HandleCS_LEAVE_ACK(msg2Handle._msg);
						break;
					case 6:
						HandleCS_DEL_ROOM_ACK(msg2Handle._msg);
						break;
					case 14:
						HandleCS_ADD_BRICK_ACK(msg2Handle._msg);
						break;
					case 16:
						HandleCS_DEL_BRICK_ACK(msg2Handle._msg);
						break;
					case 17:
						HandleCS_EDIT_BRICK_FAIL_ACK(msg2Handle._msg);
						break;
					case 18:
						HandleCS_PALETTE_ACK(msg2Handle._msg);
						break;
					case 21:
						HandleCS_CACHE_BRICK_ACK(msg2Handle._msg);
						break;
					case 22:
						HandleCS_CACHE_BRICK_DONE_ACK(msg2Handle._msg);
						break;
					case 25:
						HandleCS_CHAT_ACK(msg2Handle._msg);
						break;
					case 27:
						HandleCS_PLAYER_INFO_ACK(msg2Handle._msg);
						break;
					case 29:
						HandleCS_JOIN_ACK(msg2Handle._msg);
						break;
					case 30:
						HandleCS_UPDATE_ROOM_ACK(msg2Handle._msg);
						break;
					case 31:
						HandleCS_MASTER_ACK(msg2Handle._msg);
						break;
					case 34:
						HandleCS_ITEM_ACK(msg2Handle._msg);
						break;
					case 36:
						HandleCS_EQUIP_ACK(msg2Handle._msg);
						break;
					case 38:
						HandleCS_UNEQUIP_ACK(msg2Handle._msg);
						break;
					case 40:
						HandleCS_SAVE_ACK(msg2Handle._msg);
						break;
					case 41:
						HandleCS_USERMAP_ACK(msg2Handle._msg);
						break;
					case 43:
						HandleCS_LOAD_COMPLETE_ACK(msg2Handle._msg);
						break;
					case 45:
						HandleCS_KILL_LOG_ACK(msg2Handle._msg);
						break;
					case 46:
						HandleCS_SLOT_INFO_ACK(msg2Handle._msg);
						break;
					case 48:
						HandleCS_SET_STATUS_ACK(msg2Handle._msg);
						break;
					case 50:
						HandleCS_START_ACK(msg2Handle._msg);
						break;
					case 52:
						HandleCS_REGISTER_ACK(msg2Handle._msg);
						break;
					case 53:
						HandleCS_COPYRIGHT_ACK(msg2Handle._msg);
						break;
					case 61:
						HandleCS_REG_MAP_INFO_ACK(msg2Handle._msg);
						break;
					case 64:
						HandleCS_RESPAWN_TICKET_ACK(msg2Handle._msg);
						break;
					case 66:
						HandleCS_TIMER_ACK(msg2Handle._msg);
						break;
					case 67:
						HandleCS_TEAM_SCORE_ACK(msg2Handle._msg);
						break;
					case 68:
						HandleCS_DEATH_COUNT_ACK(msg2Handle._msg);
						break;
					case 69:
						HandleCS_KILL_COUNT_ACK(msg2Handle._msg);
						break;
					case 70:
						HandleCS_TEAM_MATCH_END_ACK(msg2Handle._msg);
						break;
					case 72:
						HandleCS_MATCH_COUNTDOWN_ACK(msg2Handle._msg);
						break;
					case 74:
						HandleCS_BREAK_INTO_ACK(msg2Handle._msg);
						break;
					case 77:
						HandleCS_DESTROY_BRICK_ACK(msg2Handle._msg);
						break;
					case 78:
						HandleCS_DESTROYED_BRICK_ACK(msg2Handle._msg);
						break;
					case 79:
						HandleCS_LOAD_START_ACK(msg2Handle._msg);
						break;
					case 84:
						HandleCS_SEED_ACK(msg2Handle._msg);
						break;
					case 81:
						HandleCS_TEAM_CHANGE_ACK(msg2Handle._msg);
						break;
					case 86:
						HandleCS_SLOT_LOCK_ACK(msg2Handle._msg);
						break;
					case 87:
						HandleCS_TEAM_CHANGE_FAIL_ACK(msg2Handle._msg);
						break;
					case 89:
						HandleCS_KICK_ACK(msg2Handle._msg);
						break;
					case 92:
						HandleCS_ROOM_CONFIG_ACK(msg2Handle._msg);
						break;
					case 96:
						HandleCS_RADIO_MSG_ACK(msg2Handle._msg);
						break;
					case 97:
						HandleCS_WHISPER_FAIL_ACK(msg2Handle._msg);
						break;
					case 99:
						HandleCS_MY_MAP_LIST_ACK(msg2Handle._msg);
						break;
					case 101:
						HandleCS_DOWNLOAD_THUMBNAIL_ACK(msg2Handle._msg);
						break;
					case 55:
						HandleCS_CHANGE_USERMAP_ALIAS_ACK(msg2Handle._msg);
						break;
					case 57:
						HandleCS_NEW_MAP_LIST_ACK(msg2Handle._msg);
						break;
					case 59:
						HandleCS_RANK_MAP_LIST_ACK(msg2Handle._msg);
						break;
					case 102:
						HandleCS_ASSET_ACK(msg2Handle._msg);
						break;
					case 104:
						HandleCS_ADD_FRIEND_ACK(msg2Handle._msg);
						break;
					case 106:
						HandleCS_ADD_BAN_ACK(msg2Handle._msg);
						break;
					case 108:
						HandleCS_DEL_FRIEND_ACK(msg2Handle._msg);
						break;
					case 110:
						HandleCS_DEL_BAN_ACK(msg2Handle._msg);
						break;
					case 113:
						HandleCS_ADD_FRIEND_FAIL_ACK(msg2Handle._msg);
						break;
					case 114:
						HandleCS_DEL_FRIEND_FAIL_ACK(msg2Handle._msg);
						break;
					case 115:
						HandleCS_ADD_BAN_FAIL_ACK(msg2Handle._msg);
						break;
					case 116:
						HandleCS_DEL_BAN_FAIL_ACK(msg2Handle._msg);
						break;
					case 117:
						HandleCS_ADD_FRIEND_BY_NICKNAME_FAIL_ACK(msg2Handle._msg);
						break;
					case 118:
						HandleCS_ADD_BAN_BY_NICKNAME_FAIL_ACK(msg2Handle._msg);
						break;
					case 119:
						HandleCS_SVC_ENTER_ACK(msg2Handle._msg);
						break;
					case 120:
						HandleCS_SVC_LEAVE_ACK(msg2Handle._msg);
						break;
					case 122:
						HandleCS_BUY_ITEM_ACK(msg2Handle._msg);
						break;
					case 124:
						HandleCS_TERM_ITEM_EXPIRED_ACK(msg2Handle._msg);
						break;
					case 125:
						HandleCS_MEMO_ACK(msg2Handle._msg);
						break;
					case 127:
						HandleCS_SEND_MEMO_ACK(msg2Handle._msg);
						break;
					case 129:
						HandleCS_PRESENT_ITEM_ACK(msg2Handle._msg);
						break;
					case 131:
						HandleCS_DEL_MEMO_ACK(msg2Handle._msg);
						break;
					case 133:
						HandleCS_RCV_PRESENT_ACK(msg2Handle._msg);
						break;
					case 136:
						HandleCS_P2P_COMPLETE_ACK(msg2Handle._msg);
						break;
					case 138:
						HandleCS_RESULT_DONE_ACK(msg2Handle._msg);
						break;
					case 141:
						HandleCS_CHANNEL_ACK(msg2Handle._msg);
						break;
					case 142:
						HandleCS_CHANNEL_END_ACK(msg2Handle._msg);
						break;
					case 146:
						HandleCS_ROAMIN_ACK(msg2Handle._msg);
						break;
					case 144:
						HandleCS_ROAMOUT_ACK(msg2Handle._msg);
						break;
					case 147:
						HandleCS_CUR_CHANNEL_ACK(msg2Handle._msg);
						break;
					case 148:
						HandleCS_PLAYER_INIT_INFO_ACK(msg2Handle._msg);
						break;
					case 149:
						HandleCS_DUPLICATE_REPORT_ACK(msg2Handle._msg);
						break;
					case 152:
						HandleCS_LEVELUP_EVENT_ACK(msg2Handle._msg);
						break;
					case 153:
						HandleCS_RENDEZVOUS_POINT_ACK(msg2Handle._msg);
						break;
					case 159:
						HandleCS_GET_CANNON_ACK(msg2Handle._msg);
						break;
					case 161:
						HandleCS_EMPTY_CANNON_ACK(msg2Handle._msg);
						break;
					case 162:
						HandleCS_XTRAP_PACKET(msg2Handle._msg);
						break;
					case 163:
						HandleCS_GOT_CANNON_ACK(msg2Handle._msg);
						break;
					case 164:
						HandleCS_DOWNLOAD_THUMBNAIL_END_ACK(msg2Handle._msg);
						break;
					case 139:
						HandleCS_ROUND_ROBIN_ACK(msg2Handle._msg);
                        
                        break;
					case 165:
						HandleCS_SERVICE_FAIL_ACK(msg2Handle._msg);
						break;
					case 166:
						HandleCS_WAITING_QUEUING_ACK(msg2Handle._msg);
						break;
					case 168:
						HandleCS_UPDATE_SCRIPT_ACK(msg2Handle._msg);
						break;
					case 169:
						HandleCS_UPDATE_SCRIPT_FAIL_ACK(msg2Handle._msg);
						break;
					case 171:
						HandleCS_TUTORIAL_COMPLETE_ACK(msg2Handle._msg);
						break;
					case 172:
						HandleCS_APS_WARNING_ACK(msg2Handle._msg);
						break;
					case 173:
						HandleCS_DOWNLOADED_MAP_ACK(msg2Handle._msg);
						break;
					case 175:
						HandleCS_DOWNLOAD_MAP_ACK(msg2Handle._msg);
						break;
					case 177:
						HandleCS_DEL_DOWNLOAD_MAP_ACK(msg2Handle._msg);
						break;
					case 179:
						HandleCS_INDIVIDUAL_SCORE_ACK(msg2Handle._msg);
						break;
					case 180:
						HandleCS_INDIVIDUAL_MATCH_END_ACK(msg2Handle._msg);
						break;
					case 182:
						HandleCS_MISSION_END_ACK(msg2Handle._msg);
						break;
					case 184:
						HandleCS_PLAYER_DETAIL_ACK(msg2Handle._msg);
						break;
					case 185:
						HandleCS_ASSIST_COUNT_ACK(msg2Handle._msg);
						break;
					case 188:
						HandleCS_CHECK_CLAN_NAME_AVAILABILITY_ACK(msg2Handle._msg);
						break;
					case 190:
						HandleCS_CREATE_CLAN_ACK(msg2Handle._msg);
						break;
					case 192:
						HandleCS_DESTROY_CLAN_ACK(msg2Handle._msg);
						break;
					case 194:
						HandleCS_SEND_CLAN_INVITATION_ACK(msg2Handle._msg);
						break;
					case 196:
						HandleCS_ANSWER_CLAN_INVITATION_ACK(msg2Handle._msg);
						break;
					case 197:
						HandleCS_RESET_MY_CLAN_ACK(msg2Handle._msg);
						break;
					case 199:
						HandleCS_LEAVE_CLAN_ACK(msg2Handle._msg);
						break;
					case 201:
						HandleCS_CLAN_MEMBER_ACK(msg2Handle._msg);
						break;
					case 202:
						HandleCS_CLAN_MEMBER_END_ACK(msg2Handle._msg);
						break;
					case 204:
						HandleCS_SELECT_CLAN_INTRO_ACK(msg2Handle._msg);
						break;
					case 205:
						HandleCS_ROUND_END_ACK(msg2Handle._msg);
						break;
					case 207:
						HandleCS_CLAN_APPLICANT_ACK(msg2Handle._msg);
						break;
					case 208:
						HandleCS_CLAN_APPLICANT_END_ACK(msg2Handle._msg);
						break;
					case 210:
						HandleCS_CLAN_KICK_ACK(msg2Handle._msg);
						break;
					case 212:
						HandleCS_UP_CLAN_MEMBER_ACK(msg2Handle._msg);
						break;
					case 214:
						HandleCS_DOWN_CLAN_MEMBER_ACK(msg2Handle._msg);
						break;
					case 216:
						HandleCS_TRANSFER_MASTER_ACK(msg2Handle._msg);
						break;
					case 218:
						HandleCS_ACCEPT_APPLICANT_ACK(msg2Handle._msg);
						break;
					case 220:
						HandleCS_OPEN_RANDOM_BOX_ACK(msg2Handle._msg);
						break;
					case 222:
						HandleCS_APPLY_CLAN_ACK(msg2Handle._msg);
						break;
					case 224:
						HandleCS_CHANGE_CLAN_INTRO_ACK(msg2Handle._msg);
						break;
					case 226:
						HandleCS_CHANGE_CLAN_NOTICE_ACK(msg2Handle._msg);
						break;
					case 228:
						HandleCS_CLAN_DETAIL_ACK(msg2Handle._msg);
						break;
					case 229:
						HandleCS_ADD_CLANEE_ACK(msg2Handle._msg);
						break;
					case 230:
						HandleCS_CLAN_INFO_CHANGE_ACK(msg2Handle._msg);
						break;
					case 231:
						HandleCS_CLAN_NEW_MEMBER_ACK(msg2Handle._msg);
						break;
					case 232:
						HandleCS_CLAN_DEL_MEMBER_ACK(msg2Handle._msg);
						break;
					case 235:
						HandleCS_CHANGE_CLAN_MARK_ACK(msg2Handle._msg);
						break;
					case 236:
						HandleCS_CLAN_MARK_CHANGED_ACK(msg2Handle._msg);
						break;
					case 238:
						HandleCS_CREATE_SQUAD_ACK(msg2Handle._msg);
						break;
					case 240:
						HandleCS_JOIN_SQUAD_ACK(msg2Handle._msg);
						break;
					case 242:
						HandleCS_ENTER_SQUAD_ACK(msg2Handle._msg);
						break;
					case 247:
						HandleCS_LEAVE_SQUAD_ACK(msg2Handle._msg);
						break;
					case 248:
						HandleCS_ADD_SQUAD_ACK(msg2Handle._msg);
						break;
					case 249:
						HandleCS_DEL_SQUAD_ACK(msg2Handle._msg);
						break;
					case 250:
						HandleCS_UPDATE_SQUAD_ACK(msg2Handle._msg);
						break;
					case 254:
						HandleCS_ENTER_SQUADING_ACK(msg2Handle._msg);
						break;
					case 256:
						HandleCS_LEAVE_SQUADING_ACK(msg2Handle._msg);
						break;
					case 258:
						HandleCS_KICK_SQUAD_ACK(msg2Handle._msg);
						break;
					case 260:
						HandleCS_CHG_SQUAD_OPTION_ACK(msg2Handle._msg);
						break;
					case 261:
						HandleCS_CLAN_MATCH_HALF_TIME_ACK(msg2Handle._msg);
						break;
					case 263:
						HandleCS_GET_BACK2SPAWNER_ACK(msg2Handle._msg);
						break;
					case 265:
						HandleCS_MATCH_RESTART_COUNT_ACK(msg2Handle._msg);
						break;
					case 267:
						HandleCS_MATCH_RESTARTED_ACK(msg2Handle._msg);
						break;
					case 269:
						HandleCS_CLAN_MATCH_RECORD_LIST_ACK(msg2Handle._msg);
						break;
					case 271:
						HandleCS_CLAN_MATCH_PLAYER_LIST_ACK(msg2Handle._msg);
						break;
					case 252:
						HandleCS_MATCH_TEAM_START_ACK(msg2Handle._msg);
						break;
					case 273:
						HandleCS_MATCH_TEAM_CANCEL_ACK(msg2Handle._msg);
						break;
					case 274:
						HandleCS_MATCH_TEAM_SUCCESS_ACK(msg2Handle._msg);
						break;
					case 276:
						HandleCS_CLAN_MATCH_TEAM_GETBACK_ACK(msg2Handle._msg);
						break;
					case 280:
						HandleCS_BM_INSTALL_BOMB_ACK(msg2Handle._msg);
						break;
					case 282:
						HandleCS_BM_UNINSTALL_BOMB_ACK(msg2Handle._msg);
						break;
					case 284:
						HandleCS_BM_BLAST_ACK(msg2Handle._msg);
						break;
					case 286:
						HandleCS_CTF_PICK_FLAG_ACK(msg2Handle._msg);
						break;
					case 288:
						HandleCS_CTF_CAPTURE_FLAG_ACK(msg2Handle._msg);
						break;
					case 290:
						HandleCS_CTF_DROP_FLAG_ACK(msg2Handle._msg);
						break;
					case 291:
						HandleCS_BLAST_MODE_END_ACK(msg2Handle._msg);
						break;
					case 292:
						HandleCS_CAPTURE_THE_FLAG_END_ACK(msg2Handle._msg);
						break;
					case 294:
						HandleCS_BLAST_MODE_SCORE_ACK(msg2Handle._msg);
						break;
					case 296:
						HandleCS_CTF_SCORE_ACK(msg2Handle._msg);
						break;
					case 298:
						HandleCS_CLAN_LIST_ACK(msg2Handle._msg);
						break;
					case 299:
						HandleCS_MISSION_COUNT_ACK(msg2Handle._msg);
						break;
					case 300:
						HandleCS_ROUND_SCORE_ACK(msg2Handle._msg);
						break;
					case 301:
						HandleCS_BM_STATUS_ACK(msg2Handle._msg);
						break;
					case 302:
						HandleCS_CTF_STATUS_ACK(msg2Handle._msg);
						break;
					case 303:
						HandleCS_ME_EDITOR_LIST_ACK(msg2Handle._msg);
						break;
					case 305:
						HandleCS_ME_CHG_EDITOR_ACK(msg2Handle._msg);
						break;
					case 308:
						HandleCS_INIT_TERM_ITEM_ACK(msg2Handle._msg);
						break;
					case 310:
						HandleCS_REBUY_ITEM_ACK(msg2Handle._msg);
						break;
					case 312:
						HandleCS_ERASE_DELETED_ITEM_ACK(msg2Handle._msg);
						break;
					case 313:
						HandleCS_ME_PREMIUM_BRICKS_ACK(msg2Handle._msg);
						break;
					case 314:
						HandleCS_WEAPON_LEVEL_ACK(msg2Handle._msg);
						break;
					case 315:
						HandleCS_BACK2BRIEFING_ACK(msg2Handle._msg);
						break;
					case 317:
						HandleCS_DISCOMPOSE_ITEM_ACK(msg2Handle._msg);
						break;
					case 318:
						HandleCS_SHOP_ACK(msg2Handle._msg);
						break;
					case 319:
						HandleCS_STAR_LEVEL_ACK(msg2Handle._msg);
						break;
					case 320:
						HandleCS_RENDEZVOUS_INFO_ACK(msg2Handle._msg);
						break;
					case 321:
						HandleCS_USERMAP_END_ACK(msg2Handle._msg);
						break;
					case 322:
						HandleCS_SHOP_END_ACK(msg2Handle._msg);
						break;
					case 323:
						HandleCS_WEAPON_MODIFIER_ACK(msg2Handle._msg);
						break;
					case 326:
						HandleCS_LINE_BRICK_ACK(msg2Handle._msg);
						break;
					case 328:
						HandleCS_REPLACE_BRICK_ACK(msg2Handle._msg);
						break;
					case 330:
						HandleCS_LINE_BRICK_FAIL_ACK(msg2Handle._msg);
						break;
					case 331:
						HandleCS_REPLACE_BRICK_FAIL_ACK(msg2Handle._msg);
						break;
					case 332:
						HandleCS_SHOOTER_TOOL_ACK(msg2Handle._msg);
						break;
					case 338:
						HandleCS_BND_MODE_END_ACK(msg2Handle._msg);
						break;
					case 339:
						HandleCS_BND_SCORE_ACK(msg2Handle._msg);
						break;
					case 340:
						HandleCS_ME_REG_BRICK_ACK(msg2Handle._msg);
						break;
					case 341:
						HandleCS_ME_REG_BRICK_END_ACK(msg2Handle._msg);
						break;
					case 343:
						HandleCS_DELETED_MAP_ACK(msg2Handle._msg);
						break;
					case 344:
						HandleCS_BND_SHIFT_PHASE_ACK(msg2Handle._msg);
						break;
					case 346:
						HandleCS_NOTICE_ACK(msg2Handle._msg);
						break;
					case 347:
						HandleCS_AD_ACK(msg2Handle._msg);
						break;
					case 350:
						HandleCS_WEAPON_DURABILITY_ACK(msg2Handle._msg);
						break;
					case 352:
						HandleCS_REPAIR_WEAPON_ACK(msg2Handle._msg);
						break;
					case 354:
						HandleCS_UPGRADE_ITEM_ACK(msg2Handle._msg);
						break;
					case 355:
						HandleCS_ITEM_PIMP_ACK(msg2Handle._msg);
						break;
					case 356:
						HandleCS_PIMP_MODIFIER_ACK(msg2Handle._msg);
						break;
					case 358:
						HandleCS_MERGE_ITEM_ACK(msg2Handle._msg);
						break;
					case 359:
						HandleCS_BUNDLE_ACK(msg2Handle._msg);
						break;
					case 361:
						HandleCS_UNPACK_BUNDLE_ACK(msg2Handle._msg);
						break;
					case 362:
						HandleCS_PROMO_LIST_ACK(msg2Handle._msg);
						break;
					case 363:
						HandleCS_BOUGHTONCE_LIST_ACK(msg2Handle._msg);
						break;
					case 365:
						HandleCS_I_AGREE_TOS_ACK(msg2Handle._msg);
						break;
					case 367:
						HandleCS_CTF_FLAG_RETURN_ACK(msg2Handle._msg);
						break;
					case 370:
						HandleCS_TC_OPEN_ACK(msg2Handle._msg);
						break;
					case 373:
						HandleCS_TC_ENTER_ACK(msg2Handle._msg);
						break;
					case 375:
						HandleCS_TC_CHEST_ACK(msg2Handle._msg);
						break;
					case 376:
						HandleCS_TC_OPEN_PRIZE_TAG_ACK(msg2Handle._msg);
						break;
					case 377:
						HandleCS_TC_TOOKOFF_ACK(msg2Handle._msg);
						break;
					case 378:
						HandleCS_TC_UPDATE_CHEST_ACK(msg2Handle._msg);
						break;
					case 380:
						HandleCS_TC_RECEIVE_PRIZE_ACK(msg2Handle._msg);
						break;
					case 381:
						HandleCS_MISSION_ACK(msg2Handle._msg);
						break;
					case 382:
						HandleCS_MISSION_WAIT_ACK(msg2Handle._msg);
						break;
					case 384:
						HandleCS_ACCEPT_DAILY_MISSION_ACK(msg2Handle._msg);
						break;
					case 387:
						HandleCS_COMPLETE_DAILY_MISSION_ACK(msg2Handle._msg);
						break;
					case 388:
						HandleCS_PROGRESS_DAILY_MISSION_ACK(msg2Handle._msg);
						break;
					case 181:
						HandleCS_CORE_HP_ACK(msg2Handle._msg);
						break;
					case 390:
						HandleCS_STARTING_ACK(msg2Handle._msg);
						break;
					case 391:
						HandleCS_TREASURE_ACK(msg2Handle._msg);
						break;
					case 394:
						HandleCS_AUTO_LOGIN_ACK(msg2Handle._msg);
						break;
					case 396:
						HandleCS_HASH_CHECK_ERROR_ACK(msg2Handle._msg);
						break;
					case 398:
						HandleCS_CHARGE_PICKNWIN_COIN_ACK(msg2Handle._msg);
						break;
					case 401:
						HandleCS_GADGET_ACK(msg2Handle._msg);
						break;
					case 403:
						HandleCS_GADGET_ACTION_ACK(msg2Handle._msg);
						break;
					case 404:
						HandleCS_GADGET_REMOVE_ACK(msg2Handle._msg);
						break;
					case 406:
						HandleCS_RESET_USER_MAP_SLOTS_ACK(msg2Handle._msg);
						break;
					case 408:
						HandleCS_INC_EXTRA_SLOTS_ACK(msg2Handle._msg);
						break;
					case 410:
						HandleCS_AUTO_LOGIN_TO_RUNUP_ACK(msg2Handle._msg);
						break;
					case 412:
						HandleCS_CREATE_CHARACTER_ACK(msg2Handle._msg);
						break;
					case 415:
						HandleCS_WEAPON_CHANGE_ACK(msg2Handle._msg);
						break;
					case 416:
						HandleCS_PLAYER_WEAPON_CHANGE_ACK(msg2Handle._msg);
						break;
					case 417:
						HandleCS_PLAYER_OPT_ACK(msg2Handle._msg);
						break;
					case 418:
						HandleCS_WEAPON_SLOT_ACK(msg2Handle._msg);
						break;
					case 421:
						HandleCS_GENERIC_BUNDLE_ACK(msg2Handle._msg);
						break;
					case 422:
						HandleCS_CUSTOM_STRING_ACK(msg2Handle._msg);
						break;
					case 424:
						HandleCS_MAP_EVAL_ACK(msg2Handle._msg);
						break;
					case 426:
						HandleCS_MY_DOWNLOAD_MAP_ACK(msg2Handle._msg);
						break;
					case 428:
						HandleCS_MY_REGISTER_MAP_ACK(msg2Handle._msg);
						break;
					case 430:
						HandleCS_USER_MAP_ACK(msg2Handle._msg);
						break;
					case 432:
						HandleCS_ALL_MAP_ACK(msg2Handle._msg);
						break;
					case 434:
						HandleCS_WEEKLY_CHART_ACK(msg2Handle._msg);
						break;
					case 436:
						HandleCS_MAP_HONOR_ACK(msg2Handle._msg);
						break;
					case 438:
						HandleCS_MAP_DETAIL_ACK(msg2Handle._msg);
						break;
					case 440:
						HandleCS_MORE_COMMENT_ACK(msg2Handle._msg);
						break;
					case 442:
						HandleCS_CHG_MAP_INTRO_ACK(msg2Handle._msg);
						break;
					case 444:
						HandleCS_CHG_MAP_DOWNLOAD_FREE_ACK(msg2Handle._msg);
						break;
					case 446:
						HandleCS_MAP_DAY_ACK(msg2Handle._msg);
						break;
					case 449:
						HandleCS_CHG_DOOR_STATUS_ACK(msg2Handle._msg);
						break;
					case 450:
						HandleCS_OPEN_DOOR_ACK(msg2Handle._msg);
						break;
					case 451:
						HandleCS_BUILDER_SLOT_ACK(msg2Handle._msg);
						break;
					case 452:
						HandleCS_WEAPON_MODIFIER_EX_ACK(msg2Handle._msg);
						break;
					case 453:
						HandleCS_ROOM_CONFIG_FAIL_ACK(msg2Handle._msg);
						break;
					case 454:
						HandleCS_SLOT_CHANGE_ACK(msg2Handle._msg);
						break;
					case 456:
						HandleCS_MUTE_ACK(msg2Handle._msg);
						break;
					case 457:
						HandleCS_YOU_ARE_MUTE_ACK(msg2Handle._msg);
						break;
					case 459:
						HandleCS_AUTO_LOGIN_TO_NETMARBLE_ACK(msg2Handle._msg);
						break;
					case 461:
						HandleCS_USERMAP_LIST_ACK(msg2Handle._msg);
						break;
					case 462:
						HandleCS_SHOOTER_TOOL_LIST_ACK(msg2Handle._msg);
						break;
					case 463:
						HandleCS_WEAPON_SLOT_LIST_ACK(msg2Handle._msg);
						break;
					case 464:
						HandleCS_ITEM_LIST_ACK(msg2Handle._msg);
						break;
					case 465:
						HandleCS_DOWNLOADED_MAP_LIST_ACK(msg2Handle._msg);
						break;
					case 466:
						HandleCS_DELETED_MAP_LIST_ACK(msg2Handle._msg);
						break;
					case 467:
						HandleCS_SVC_ENTER_LIST_ACK(msg2Handle._msg);
						break;
					case 468:
						HandleCS_ROOM_LIST_ACK(msg2Handle._msg);
						break;
					case 470:
						HandleCS_ROOM_ACK(msg2Handle._msg);
						break;
					case 472:
						HandleCS_CHARGE_FORCE_POINT_ACK(msg2Handle._msg);
						break;
					case 473:
						HandleCS_XP_ACK(msg2Handle._msg);
						break;
					case 475:
						HandleCS_BUNGEE_SCORE_ACK(msg2Handle._msg);
						break;
					case 476:
						HandleCS_BUNGEE_MODE_END_ACK(msg2Handle._msg);
						break;
					case 477:
						HandleCS_CUR_CHANNEL_SPECIFIC_INFO_AC(msg2Handle._msg);
						break;
					case 480:
						HandleCS_BATCH_DEL_BRICK_ACK(msg2Handle._msg);
						break;
					case 482:
						HandleCS_INVITE_ACK(msg2Handle._msg);
						break;
					case 483:
						HandleCS_INVITED_ACK(msg2Handle._msg);
						break;
					case 485:
						HandleCS_FOLLOWING_ACK(msg2Handle._msg);
						break;
					case 486:
						HandleCS_NMPLAYAUTH_STATE_NOTI_ACK(msg2Handle._msg);
						break;
					case 487:
						HandleCS_CPTR_ACK(msg2Handle._msg);
						break;
					case 488:
						HandleCS_CPTR_REWARD_ACK(msg2Handle._msg);
						break;
					case 489:
						HandleCS_LEVELUP_REWARD_ACK(msg2Handle._msg);
						break;
					case 490:
						HandleCS_MISSION_REWARD_ACK(msg2Handle._msg);
						break;
					case 491:
						HandleCS_ITEM_PROPERTY_ACK(msg2Handle._msg);
						break;
					case 492:
						HandleCS_PREMIUM_ITEM_ACK(msg2Handle._msg);
						break;
					case 493:
						HandleCS_NETCAFE_ITEM_ACK(msg2Handle._msg);
						break;
					case 495:
						HandleCS_START_KICKOUT_VOTE_ACK(msg2Handle._msg);
						break;
					case 497:
						HandleCS_KICKOUT_VOTE_ACK(msg2Handle._msg);
						break;
					case 498:
						HandleCS_KICKOUT_VOTE_STATUS_ACK(msg2Handle._msg);
						break;
					case 499:
						HandleCS_KICKOUT_VOTE_END_ACK(msg2Handle._msg);
						break;
					case 500:
						HandleCS_CUSTOM_GAME_CONFIG_ACK(msg2Handle._msg);
						break;
					case 502:
						HandleCS_USE_CHANGE_NICKNAME_ACK(msg2Handle._msg);
						break;
					case 503:
						HandleCS_NICKNAME_CHANGE_ACK(msg2Handle._msg);
						break;
					case 505:
						HandleCS_LOGIN_TO_AXESO5_ACK(msg2Handle._msg);
						break;
					case 506:
						HandleCS_BND_STATUS_ACK(msg2Handle._msg);
						break;
					case 507:
						HandleCS_BUNGEE_STATUS_ACK(msg2Handle._msg);
						break;
					case 509:
						HandleCS_MISSION_POINT_ACK(msg2Handle._msg);
						break;
					case 511:
						HandleCS_ACCUSE_PLAYER_ACK(msg2Handle._msg);
						break;
					case 513:
						HandleCS_ACCUSE_MAP_ACK(msg2Handle._msg);
						break;
					case 515:
						HandleCS_RESET_BATTLE_RECORD_ACK(msg2Handle._msg);
						break;
					case 518:
						HandleCS_RANDOM_INVITE_ACK(msg2Handle._msg);
						break;
					case 520:
						HandleCS_ESCAPE_SCORE_ACK(msg2Handle._msg);
						break;
					case 521:
						HandleCS_ESCAPE_MODE_END_ACK(msg2Handle._msg);
						break;
					case 524:
						HandleCS_ESCAPE_GOAL_ACK(msg2Handle._msg);
						break;
					case 527:
						HandleCS_DROP_ITEM_ACK(msg2Handle._msg);
						break;
					case 522:
						HandleCS_ESCAPE_STATUS_ACK(msg2Handle._msg);
						break;
					case 529:
						HandleCS_PICKUP_DROPPED_ITEM_ACK(msg2Handle._msg);
						break;
					case 530:
						HandleCS_DEL_DROP_ITEM_ACK(msg2Handle._msg);
						break;
					case 531:
						HandleCS_DROPITEM_QUICK_JOIN_USER_ACK(msg2Handle._msg);
						break;
					case 532:
						HandleCS_STAR_RATE_ACK(msg2Handle._msg);
						break;
					case 533:
						HandleCS_PICKUP_DROPPED_ITEM_FAIL_ACK(msg2Handle._msg);
						break;
					case 535:
						HandleCS_MASTER_KICKING_ACK(msg2Handle._msg);
						break;
					case 536:
						HandleCS_ZOMBIE_MODE_SCORE_ACK(msg2Handle._msg);
						break;
					case 537:
						HandleCS_ZOMBIE_END_ACK(msg2Handle._msg);
						break;
					case 539:
						HandleCS_ZOMBIE_INFECTION_ACK(msg2Handle._msg);
						break;
					case 541:
						HandleCS_ZOMBIE_INFECT_ACK(msg2Handle._msg);
						break;
					case 542:
						HandleCS_SELECT_WANTED_ACK(msg2Handle._msg);
						break;
					case 543:
						HandleCS_KNOW_WANTED_ACK(msg2Handle._msg);
						break;
					case 544:
						HandleCS_DESELECT_WANTED_ACK(msg2Handle._msg);
						break;
					case 546:
						HandleCS_ZOMBIE_BREAK_INTO_ACK(msg2Handle._msg);
						break;
					case 548:
						HandleCS_ZOMBIE_STATUS_ACK(msg2Handle._msg);
						break;
					case 550:
						HandleCS_ADD_AMMO_ACK(msg2Handle._msg);
						break;
					case 552:
						HandleCS_GET_TRAIN_ACK(msg2Handle._msg);
						break;
					case 554:
						HandleCS_EMPTY_TRAIN_ACK(msg2Handle._msg);
						break;
					case 556:
						HandleCS_CLAN_APPLY_LIST_ACK(msg2Handle._msg);
						break;
					case 558:
						HandleCS_CLAN_CANCEL_APPLICATION_ACK(msg2Handle._msg);
						break;
					case 559:
						HandleCS_CLAN_APPLICANT_COUNT_ACK(msg2Handle._msg);
						break;
					case 561:
						HandleCS_CLAN_MATCHING_LIST_ACK(msg2Handle._msg);
						break;
					case 563:
						HandleCS_CLAN_NEED_CREATE_POINT_ACK(msg2Handle._msg);
						break;
					case 564:
						HandleCS_CLAN_CHANGE_ROOM_NAME_ACK(msg2Handle._msg);
						break;
					case 565:
						HandleCS_CLAN_GET_OUT_SQUAD_ACK(msg2Handle._msg);
						break;
					case 277:
						HandleCS_MATCH_TEAM_CANCELED_ACK(msg2Handle._msg);
						break;
					default:
						Debug.LogError("Unknown msg encountered" + msg2Handle._id);
						break;
					}
					if (_readQueue != null && _readQueue.Count > 0)
					{
						_readQueue.Dequeue();
					}
				}
			}
		}
	}

	public bool IsActive()
	{
		return IsConnected() && _heartbeat;
	}

	public bool IsConnected()
	{
		if (_sock == null)
		{
			return false;
		}
		return _sock.Connected;
	}

	public void Say(ushort id, MsgBody msgBody)
	{
		if (_sock != null && _writeQueue != null)
		{
			Msg4Send msg4Send = new Msg4Send(id, uint.MaxValue, uint.MaxValue, msgBody, GetSendKey());
			lock (this)
			{
				if (_writeQueue.Count > 0)
				{
					_writeQueue.Enqueue(msg4Send);
				}
				else
				{
					_writeQueue.Enqueue(msg4Send);
					try
					{
						_sock.BeginSend(msg4Send.Buffer, 0, msg4Send.Buffer.Length, SocketFlags.None, SendCallback, null);
					}
					catch (Exception ex)
					{
						Debug.LogError("Error, " + ex.Message.ToString());
						Close();
					}
				}
			}
		}
	}

	private void SendCallback(IAsyncResult ar)
	{
		if (_sock != null)
		{
			try
			{
				int num = _sock.EndSend(ar);
				lock (this)
				{
					Msg4Send msg4Send = (Msg4Send)_writeQueue.Peek();
					msg4Send.Io += num;
					if (msg4Send.GetStatus() == Msg4Send.MsgStatus.COMPLETE)
					{
						_writeQueue.Dequeue();
					}
					if (_writeQueue.Count > 0)
					{
						msg4Send = (Msg4Send)_writeQueue.Peek();
						_sock.BeginSend(msg4Send.Buffer, msg4Send.Io, msg4Send.Buffer.Length - msg4Send.Io, SocketFlags.None, SendCallback, null);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Error, " + ex.Message.ToString());
				Close();
			}
		}
	}

	public void SendCS_ZOMBIE_STATUS_REQ(int status, int time, int cntDn)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(status);
		msgBody.Write(time);
		msgBody.Write(cntDn);
		Say(547, msgBody);
	}

	public void SendCS_ZOMBIE_OBSERVER_REQ()
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(MyInfoManager.Instance.Seq);
		Say(549, msgBody);
	}

	public void SendCS_ZOMBIE_MODE_SCORE_REQ()
	{
		MsgBody msgBody = new MsgBody();
		Say(545, msgBody);
	}

	public void SendCS_ZOMBIE_INFECTION_REQ()
	{
		MsgBody msgBody = new MsgBody();
		Say(538, msgBody);
	}

	public void SendCS_ZOMBIE_INFECT_REQ(int brickMan, int zombie)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(brickMan);
		msgBody.Write(zombie);
		Say(540, msgBody);
	}

	public void SendCS_MUTE_REQ(string nickname, int howlong)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(nickname);
		msgBody.Write(howlong);
		if (howlong == 0)
		{
			GM_COMMAND_LOGER.SendLog(GM_COMMAND_LOGER.GM_COMMAND_LOG.MUTE_OFF);
		}
		else
		{
			GM_COMMAND_LOGER.SendLog(GM_COMMAND_LOGER.GM_COMMAND_LOG.MUTE_ON);
		}
		Say(455, msgBody);
	}

	public void SendCS_SAVE_PLAYER_COMMON_OPT_REQ(int commonOpt)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(commonOpt);
		Say(460, msgBody);
	}

	public void SendCS_SET_WEAPON_SLOT_REQ(int slot, long weapon)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(slot);
		msgBody.Write(weapon);
		Say(419, msgBody);
	}

	public void SendCS_CLEAR_WEAPON_SLOT_REQ()
	{
		Say(420, new MsgBody());
	}

	public void SendCS_WEAPON_CHANGE_REQ(int slot, long seq, string next, string prev)
	{
		if (!_waitingAck)
		{
			MsgBody msgBody = new MsgBody();
			msgBody.Write(slot);
			msgBody.Write(seq);
			msgBody.Write(next);
			msgBody.Write(prev);
			Say(414, msgBody);
			_waitingAck = true;
		}
	}

	public void SendCS_CREATE_CHARACTER_REQ(string nickname)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(nickname);
		Say(411, msgBody);
	}

	public void SendCS_COMPLETE_DAILY_MISSION_REQ()
	{
		if (!_waitingAck)
		{
			Say(386, new MsgBody());
			_waitingAck = false;
		}
	}

	public void SendCS_GIVEUP_DAILY_MISSION_REQ()
	{
		Say(385, new MsgBody());
	}

	public void SendCS_ACCEPT_DAILY_MISSION_REQ()
	{
		if (!_waitingAck)
		{
			Say(383, new MsgBody());
			_waitingAck = true;
		}
	}

	public void SendCS_TC_OPEN_REQ()
	{
		if (!_waitingAck)
		{
			Say(369, new MsgBody());
			_waitingAck = true;
		}
	}

	public void SendCS_TC_CLOSE_REQ()
	{
		Say(371, new MsgBody());
	}

	public void SendCS_TC_ENTER_REQ(int chest)
	{
		if (!_waitingAck)
		{
			MsgBody msgBody = new MsgBody();
			msgBody.Write(chest);
			Say(372, msgBody);
			_waitingAck = true;
		}
	}

	public void SendCS_TC_OPEN_PRIZE_TAG_REQ(int chest, int index, bool freeCoin)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(chest);
		msgBody.Write(index);
		msgBody.Write(freeCoin);
		Say(374, msgBody);
	}

	public void SendCS_TC_RECEIVE_PRIZE_REQ(long item, int index, int orgAmount, bool wasKey, bool freeCoin)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(item);
		msgBody.Write(index);
		msgBody.Write(orgAmount);
		msgBody.Write(wasKey);
		msgBody.Write(freeCoin);
		Say(379, msgBody);
	}

	public void SendCS_TC_LEAVE_REQ()
	{
		Say(392, new MsgBody());
	}

	public void SendCS_I_AGREE_TOS_REQ()
	{
		Say(364, new MsgBody());
	}

	public void SendCS_RESET_USER_MAP_SLOTS_REQ(int slot, long item, string itemCode)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(slot);
		msgBody.Write(item);
		msgBody.Write(itemCode);
		Say(405, msgBody);
	}

	public void SendCS_INC_EXTRA_SLOTS_REQ(long item, string itemCode)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(item);
		msgBody.Write(itemCode);
		Say(407, msgBody);
	}

	public void SendCS_UNPACK_BUNDLE_REQ(long bundle, string bundleCode)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(bundle);
		msgBody.Write(bundleCode);
		Say(360, msgBody);
	}

	public void SendCS_WEAPON_HELD_RATIO_REQ(Dictionary<long, float> weaponHeldRatio)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(weaponHeldRatio.Count);
		foreach (KeyValuePair<long, float> item in weaponHeldRatio)
		{
			msgBody.Write(item.Key);
			msgBody.Write(item.Value);
		}
		Say(368, msgBody);
	}

	public void SendCS_CTF_FLAG_RETURN_REQ(float x, float y, float z)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(x);
		msgBody.Write(y);
		msgBody.Write(z);
		Say(366, msgBody);
	}

	public void SendCS_STACK_POINT_REQ()
	{
		Say(345, new MsgBody());
	}

	public void SendCS_REPAIR_WEAPON_REQ(long item, string code, int buyHow, int repairFee)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(item);
		msgBody.Write(code);
		msgBody.Write(buyHow);
		msgBody.Write(repairFee);
		Say(351, msgBody);
	}

	public void SendCS_UPGRADE_ITEM_REQ(long item, string code, long upgrader, string upgradercode, int prop)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(item);
		msgBody.Write(code);
		msgBody.Write(upgrader);
		msgBody.Write(upgradercode);
		msgBody.Write(prop);
		Say(353, msgBody);
	}

	public void SendCS_MERGE_ITEM_REQ(long src, long dst, string code)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(src);
		msgBody.Write(dst);
		msgBody.Write(code);
		Say(357, msgBody);
	}

	public void SendCS_REPLACE_BRICK_REQ(long item, string code, int existing, byte brick, byte x, byte y, byte z, byte rot)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(item);
		msgBody.Write(code);
		msgBody.Write(existing);
		msgBody.Write(brick);
		msgBody.Write(x);
		msgBody.Write(y);
		msgBody.Write(z);
		msgBody.Write(rot);
		Say(327, msgBody);
	}

	public void SendCS_LINE_BRICK_REQ(long item, string code, byte brick, byte x, byte y, byte z, byte rot)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(item);
		msgBody.Write(code);
		msgBody.Write(brick);
		msgBody.Write(x);
		msgBody.Write(y);
		msgBody.Write(z);
		msgBody.Write(rot);
		Say(325, msgBody);
	}

	public void SendCS_INIT_TERM_ITEM_REQ(long item, string code)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(item);
		msgBody.Write(code);
		Say(307, msgBody);
	}

	public void SendCS_DISCOMPOSE_ITEM_REQ(long item, string code, int opt)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(item);
		msgBody.Write(code);
		msgBody.Write(opt);
		Say(316, msgBody);
	}

	public void SendCS_CLAN_LIST_REQ(int prev, int next, int index, string keyword)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(prev);
		msgBody.Write(next);
		msgBody.Write(index);
		msgBody.Write(keyword);
		Say(297, msgBody);
	}

	public void SendCS_BLAST_MODE_SCORE_REQ()
	{
		Say(293, new MsgBody());
	}

	public void SendCS_DELEGATE_MASTER_REQ(int newMater)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(newMater);
		Say(389, msgBody);
	}

	public void SendCS_CTF_SCORE_REQ()
	{
		Say(295, new MsgBody());
	}

	public void SendCS_CTF_PICK_FLAG_REQ(int flag)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(flag);
		Say(285, msgBody);
	}

	public void SendCS_CTF_CAPTURE_FLAG_REQ(int flag, bool opponent)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(flag);
		msgBody.Write(opponent);
		Say(287, msgBody);
	}

	public void SendCS_CTF_DROP_FLAG_REQ(float x, float y, float z)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(x);
		msgBody.Write(y);
		msgBody.Write(z);
		Say(289, msgBody);
	}

	public void SendCS_BM_INSTALL_BOMB_REQ(int bomb, Vector3 pos, Vector3 normal)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(bomb);
		msgBody.Write(pos.x);
		msgBody.Write(pos.y);
		msgBody.Write(pos.z);
		msgBody.Write(normal.x);
		msgBody.Write(normal.y);
		msgBody.Write(normal.z);
		Say(279, msgBody);
	}

	public void SendCS_USE_SHOOTER_CONSUMABLE_REQ(long item, string code)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(item);
		msgBody.Write(code);
		Say(329, msgBody);
	}

	public void SendCS_BM_UNINSTALL_BOMB_REQ(int bomb)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(bomb);
		Say(281, msgBody);
	}

	public void SendCS_BM_BLAST_REQ(int bomb)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(bomb);
		Say(283, msgBody);
	}

	public void SendCS_UPDATE_SQUAD_RECORD_REQ(int win, int draw, int lose)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(win);
		msgBody.Write(draw);
		msgBody.Write(lose);
		Say(278, msgBody);
	}

	public void SendCS_CLAN_MATCH_TEAM_GETBACK_REQ(int clan, int index)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(clan);
		msgBody.Write(index);
		Say(275, msgBody);
	}

	public void SendCS_QUICK_JOIN_REQ(int qjModeMask, int qjOfficialMask)
	{
		BrickManManager.Instance.Clear();
		MsgBody msgBody = new MsgBody();
		msgBody.Write(qjModeMask);
		msgBody.Write(qjOfficialMask);
		Say(9, msgBody);
	}

	public void SendCS_MATCH_TEAM_START_REQ(int mode, int maxPlayer, int map, int killCount, int timeLimit, int weaponOption, string alias, bool wanted)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(mode);
		msgBody.Write(maxPlayer);
		msgBody.Write(map);
		msgBody.Write(killCount);
		msgBody.Write(timeLimit);
		msgBody.Write(weaponOption);
		msgBody.Write(alias);
		msgBody.Write(wanted);
		Say(251, msgBody);
	}

	public void SendCS_MATCH_TEAM_CANCEL_REQ()
	{
		Say(272, new MsgBody());
	}

	public void SendCS_CORE_HP_REQ(int redHp, int blueHp)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(redHp);
		msgBody.Write(blueHp);
		Say(342, msgBody);
	}

	public void SendCS_CLAN_MATCH_PLAYER_LIST_REQ(long clanMatch)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(clanMatch);
		Say(270, msgBody);
	}

	public void SendCS_CLAN_MATCH_RECORD_LIST_REQ(int prevPage, int nextPage, long index, int clan)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(prevPage);
		msgBody.Write(nextPage);
		msgBody.Write(index);
		msgBody.Write(clan);
		Say(268, msgBody);
	}

	public void SendCS_MATCH_RESTARTED_REQ()
	{
		Say(266, new MsgBody());
	}

	public void SendCS_GET_BACK2SPAWNER_REQ()
	{
		Say(262, new MsgBody());
	}

	public void SendCS_MATCH_RESTART_COUNT_REQ(int count)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(count);
		Say(264, msgBody);
	}

	public void SendCS_CHG_SQUAD_OPTION_REQ(int wannaPlayMap, int wannaPlayMode, int maxPlayers)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(wannaPlayMap);
		msgBody.Write(wannaPlayMode);
		msgBody.Write(maxPlayers);
		Say(259, msgBody);
	}

	public void SendCS_KICK_SQUAD_REQ(int kicked)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(kicked);
		Say(257, msgBody);
	}

	public bool SendCS_ENTER_SQUADING_REQ()
	{
		Say(253, new MsgBody());
		return true;
	}

	public void SendCS_LEAVE_SQUADING_REQ()
	{
		Say(255, new MsgBody());
	}

	public void SendCS_CREATE_SQUAD_REQ(int clan, int wannaPlanyMap, int wannaPlayMode, int maxMember)
	{
		if (!_waitingAck)
		{
			MsgBody msgBody = new MsgBody();
			msgBody.Write(clan);
			msgBody.Write(wannaPlanyMap);
			msgBody.Write(wannaPlayMode);
			msgBody.Write(maxMember);
			Say(237, msgBody);
			_waitingAck = false;
		}
	}

	public void SendCS_JOIN_SQUAD_REQ(int clan, int index, int squadCounter)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(clan);
		msgBody.Write(index);
		msgBody.Write(squadCounter);
		Say(239, msgBody);
	}

	public void SendCS_LEAVE_SQUAD_REQ()
	{
		Say(241, new MsgBody());
	}

	public void SendCS_CLAN_DETAIL_REQ(int clan)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(clan);
		Say(227, msgBody);
	}

	public void SendCS_CHANGE_CLAN_MARK_REQ(int clan, int mark, long ticket, string ticketCode)
	{
		if (!_waitingAck)
		{
			MsgBody msgBody = new MsgBody();
			msgBody.Write(clan);
			msgBody.Write(mark);
			msgBody.Write(ticket);
			msgBody.Write(ticketCode);
			Say(234, msgBody);
			_waitingAck = true;
		}
	}

	public void SendCS_CHANGE_CLAN_INTRO_REQ(int clan, string intro)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(clan);
		msgBody.Write(intro);
		Say(223, msgBody);
	}

	public void SendCS_CHANGE_CLAN_NOTICE_REQ(int clan, string notice)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(clan);
		msgBody.Write(notice);
		Say(225, msgBody);
	}

	public void SendCS_APPLY_CLAN_REQ(int clan, string clanName)
	{
		if (!_waitingAck)
		{
			MsgBody msgBody = new MsgBody();
			msgBody.Write(clan);
			msgBody.Write(clanName);
			Say(221, msgBody);
			_waitingAck = true;
		}
	}

	public void SendCS_BND_SHIFT_PHASE_REQ(int repeat, bool isBuildPhase)
	{
        Debug.LogWarning("Send BND # Req");
		MsgBody msgBody = new MsgBody();
        Debug.LogWarning(repeat);
		msgBody.Write(repeat);
        Debug.LogWarning(isBuildPhase);
		msgBody.Write(isBuildPhase);
		Say(349, msgBody);
	}

	public void SendCS_CHG_COUNTRY_FILTER_REQ(int country)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(country);
		Say(348, msgBody);
	}

	public void SendCS_REG_MAP_INFO_REQ(int map)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(map);
		Say(337, msgBody);
	}

	public void SendCS_CLAN_KICK_REQ(int clan, int kick, string nickname, string title, string contents)
	{
		if (!_waitingAck)
		{
			MsgBody msgBody = new MsgBody();
			msgBody.Write(clan);
			msgBody.Write(kick);
			msgBody.Write(nickname);
			msgBody.Write(title);
			msgBody.Write(contents);
			Say(209, msgBody);
			_waitingAck = true;
		}
	}

	public void SendCS_UP_CLAN_MEMBER_REQ(int clan, int member, string nickname, string title, string contents)
	{
		if (!_waitingAck)
		{
			MsgBody msgBody = new MsgBody();
			msgBody.Write(clan);
			msgBody.Write(member);
			msgBody.Write(nickname);
			msgBody.Write(title);
			msgBody.Write(contents);
			Say(211, msgBody);
			_waitingAck = true;
		}
	}

	public void SendCS_DOWN_CLAN_MEMBER_REQ(int clan, int member, string nickname, string title, string contents)
	{
		if (!_waitingAck)
		{
			MsgBody msgBody = new MsgBody();
			msgBody.Write(clan);
			msgBody.Write(member);
			msgBody.Write(nickname);
			msgBody.Write(title);
			msgBody.Write(contents);
			Say(213, msgBody);
			_waitingAck = true;
		}
	}

	public void SendCS_TRANSFER_MASTER_REQ(int clan, int member, string nickname, string title, string contents)
	{
		if (!_waitingAck)
		{
			MsgBody msgBody = new MsgBody();
			msgBody.Write(clan);
			msgBody.Write(member);
			msgBody.Write(nickname);
			msgBody.Write(title);
			msgBody.Write(contents);
			Say(215, msgBody);
			_waitingAck = true;
		}
	}

	public void SendCS_ACCEPT_APPLICANT_REQ(int clan, int member, string nickname, bool accept, string title, string contents)
	{
		if (!_waitingAck)
		{
			MsgBody msgBody = new MsgBody();
			msgBody.Write(clan);
			msgBody.Write(member);
			msgBody.Write(nickname);
			msgBody.Write(accept);
			msgBody.Write(title);
			msgBody.Write(contents);
			Say(217, msgBody);
			_waitingAck = true;
		}
	}

	public void SendCS_SELECT_CLAN_INTRO_REQ(int clan)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(clan);
		Say(203, msgBody);
	}

	public void SendCS_SELECT_CLAN_APPLICANT_REQ(int clan)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(clan);
		Say(206, msgBody);
	}

	public void SendCS_SELECT_CLAN_MEMBER_REQ(int clan)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(clan);
		Say(200, msgBody);
	}

	public void SendCS_SEND_CLAN_INVITATION_REQ(int inviteeSeq, string invitee, string title, string contents)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(inviteeSeq);
		msgBody.Write(invitee);
		msgBody.Write(title);
		msgBody.Write(contents);
		Say(193, msgBody);
	}

	public void SendCS_DESTROY_CLAN_REQ(int clan)
	{
		if (!_waitingAck)
		{
			MsgBody msgBody = new MsgBody();
			msgBody.Write(clan);
			Say(191, msgBody);
			_waitingAck = true;
		}
	}

	public void SendCS_CHECK_CLAN_NAME_AVAILABILITY_REQ(string clanName)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(clanName);
		Say(187, msgBody);
	}

	public void SendCS_CREATE_CLAN_REQ(string clanName)
	{
		if (!_waitingAck)
		{
			MsgBody msgBody = new MsgBody();
			msgBody.Write(clanName);
			Say(189, msgBody);
			_waitingAck = true;
		}
	}

	public void SendCS_OPEN_RANDOM_BOX_REQ(int randombox)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(randombox);
		Say(219, msgBody);
	}

	public void SendCS_TUTORIAL_COMPLETE_REQ(byte endOf)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(endOf);
		Say(170, msgBody);
	}

	public void SendCS_UPDATE_SCRIPT_REQ(int seq, string alias, bool enableOnAwake, bool visibleOnAwake, string commands)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(seq);
		msgBody.Write(alias);
		msgBody.Write(enableOnAwake);
		msgBody.Write(visibleOnAwake);
		msgBody.Write(commands);
		Say(167, msgBody);
	}

	public void SendCS_EMPTY_CANNON_REQ(int cannon)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(cannon);
		Say(160, msgBody);
	}

	public void SendCS_EMPTY_TRAIN_REQ(int trainSeq)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(trainSeq);
		Say(553, msgBody);
	}

	public void SendCS_CLAN_APPLY_LIST_REQ()
	{
		MsgBody msgBody = new MsgBody();
		Say(555, msgBody);
	}

	public void SendCS_CLAN_CANCEL_APPLICATION_REQ(string clanName)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(clanName);
		Say(557, msgBody);
	}

	public void SendCS_CLAN_MATCHING_LIST_REQ()
	{
		MsgBody msgBody = new MsgBody();
		Say(560, msgBody);
	}

	public void SendCS_CLAN_NEED_CREATE_POINT_REQ()
	{
		MsgBody msgBody = new MsgBody();
		Say(562, msgBody);
	}

	public void SendCS_GET_CANNON_REQ(int cannon)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(cannon);
		Say(158, msgBody);
	}

	public void SendCS_GET_TRAIN_REQ(int trainSeq, int trainID)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(trainSeq);
		msgBody.Write(trainID);
		Say(551, msgBody);
	}

	public void SendCS_SEND_MEMO_REQ(string receiver, string title, string contents)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(receiver);
		msgBody.Write(title);
		msgBody.Write(contents);
		Say(126, msgBody);
	}

	public void SendCS_PRESENT_ITEM_REQ(string code, int buyHow, int option, string receiver, string title, string contents)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(code);
		msgBody.Write(buyHow);
		msgBody.Write(option);
		msgBody.Write(receiver);
		msgBody.Write(title);
		msgBody.Write(contents);
		Say(128, msgBody);
	}

	public void SendCS_LEAVE_CLAN_REQ()
	{
		if (!_waitingAck)
		{
			Say(198, new MsgBody());
			_waitingAck = true;
		}
	}

	public void SendCS_ANSWER_CLAN_INVITATION_REQ(long memoSeq, int clan, bool accept, string title, string contents)
	{
		if (!_waitingAck)
		{
			MsgBody msgBody = new MsgBody();
			msgBody.Write(memoSeq);
			msgBody.Write(clan);
			msgBody.Write(accept);
			msgBody.Write(title);
			msgBody.Write(contents);
			Say(195, msgBody);
			_waitingAck = true;
		}
	}

	public void SendCS_READ_MEMO_REQ(long seq)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(seq);
		Say(134, msgBody);
	}

	public void SendCS_ADD_FRIEND_REQ(int friendWannabe)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(friendWannabe);
		Say(103, msgBody);
	}

	public void SendCS_LEVELUP_EVENT_REQ()
	{
		Say(151, new MsgBody());
	}

	public void SendCS_ADD_BAN_REQ(int banWannabe)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(banWannabe);
		Say(105, msgBody);
	}

	public void SendCS_RCV_PRESENT_REQ(long memoSeq, string attached, int option, bool isAmount)
	{
		if (!_waitingAck)
		{
			byte val = (byte)(isAmount ? 1 : 0);
			MsgBody msgBody = new MsgBody();
			msgBody.Write(memoSeq);
			msgBody.Write(attached);
			msgBody.Write(option);
			msgBody.Write(val);
			Say(132, msgBody);
			_waitingAck = true;
		}
	}

	public void SendCS_DEL_FRIEND_REQ(int seq)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(seq);
		Say(107, msgBody);
	}

	public void SendCS_DEL_BAN_REQ(int seq)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(seq);
		Say(109, msgBody);
	}

	public void SendCS_RESULT_DONE_REQ()
	{
		Say(137, new MsgBody());
	}

	public void SendCS_DOWNLOAD_THUMBNAIL_REQ(bool isUserMap, int seq)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write((byte)(isUserMap ? 1 : 0));
		msgBody.Write(seq);
		Say(100, msgBody);
	}

	public void SendCS_DEL_MEMO_REQ(long seq)
	{
		if (!_waitingAck)
		{
			MsgBody msgBody = new MsgBody();
			msgBody.Write(seq);
			Say(130, msgBody);
			_waitingAck = true;
		}
	}

	public void SendCS_P2P_COMPLETE_REQ()
	{
		Say(135, new MsgBody());
	}

	public void SendCS_BUY_ITEM_REQ(string code, int buyHow, int option, bool isAmount, bool needEqup)
	{
		MsgBody msgBody = new MsgBody();
		byte val = (byte)(isAmount ? 1 : 0);
		msgBody.Write(code);
		msgBody.Write(buyHow);
		msgBody.Write(option);
		msgBody.Write(val);
		msgBody.Write(needEqup);
		Say(121, msgBody);
	}

	public void SendCS_ERASE_DELETED_ITEM_REQ(long item, string code)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(item);
		msgBody.Write(code);
		Say(311, msgBody);
	}

	public void SendCS_REBUY_ITEM_REQ(long itemSeq, string code, int buyHow, int option, bool isAmount, bool needEqup)
	{
		MsgBody msgBody = new MsgBody();
		byte val = (byte)(isAmount ? 1 : 0);
		msgBody.Write(itemSeq);
		msgBody.Write(code);
		msgBody.Write(buyHow);
		msgBody.Write(option);
		msgBody.Write(val);
		msgBody.Write(needEqup);
		Say(309, msgBody);
	}

	public void SendCS_SLOT_LOCK_REQ(sbyte slot, sbyte lck)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(slot);
		msgBody.Write(lck);
		Say(85, msgBody);
	}

	public void SendCS_GM_SAYS_REQ(string text)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(text);
		Say(186, msgBody);
	}

	public void SendCS_PLAYER_DETAIL_REQ(int seq)
	{
		if (!_waitingAck)
		{
			MsgBody msgBody = new MsgBody();
			msgBody.Write(seq);
			Say(183, msgBody);
			_waitingAck = true;
		}
	}

	public void SendCS_RANK_MAP_LIST_REQ(int prevPage, int nextPage, int indexer, byte modeMask)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(prevPage);
		msgBody.Write(nextPage);
		msgBody.Write(indexer);
		msgBody.Write(modeMask);
		Say(58, msgBody);
	}

	public void SendCS_MY_MAP_LIST_REQ(int prevPage, int nextPage, int indexer, byte modeMask)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(prevPage);
		msgBody.Write(nextPage);
		msgBody.Write(indexer);
		msgBody.Write(modeMask);
		Say(98, msgBody);
	}

	public void SendCS_SET_SHOOTER_TOOL_REQ(sbyte slot, long tool)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(slot);
		msgBody.Write(tool);
		Say(333, msgBody);
	}

	public void SendCS_CLEAR_SHOOTER_TOOLS_REQ()
	{
		Say(334, new MsgBody());
	}

	public void SendCS_NEW_MAP_LIST_REQ(int prevPage, int nextPage, int indexer, byte modeMask, sbyte flag, string filter)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(prevPage);
		msgBody.Write(nextPage);
		msgBody.Write(indexer);
		msgBody.Write(modeMask);
		msgBody.Write(flag);
		msgBody.Write(filter);
		Say(56, msgBody);
	}

	public void SendCS_ROOM_CONFIG_REQ(int killCount, int timeLimit, int weaponOption, int nWhere, int breakInto, int teamBalance, int itemPickup, int useBuildGun, string whereAlias, string pswd, int type)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(killCount);
		msgBody.Write(timeLimit);
		msgBody.Write(weaponOption);
		msgBody.Write(nWhere);
		msgBody.Write(breakInto);
		msgBody.Write(teamBalance);
		msgBody.Write(useBuildGun);
		msgBody.Write(itemPickup);
		msgBody.Write(whereAlias);
		msgBody.Write(pswd);
		msgBody.Write(type);
		Say(91, msgBody);
	}

	public void SendCS_STAR_DUST_REQ(int map, int starDust)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(map);
		msgBody.Write(starDust);
		Say(60, msgBody);
	}

	public void SendCS_SERIAL_BONUS_REQ(int bonus)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(bonus);
		Say(82, msgBody);
	}

	public void SendCS_TIMER_REQ(int remainTime, int playTime)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(remainTime);
		msgBody.Write(playTime);
		Say(65, msgBody);
	}

	public void SendCS_CHANGE_USERMAP_ALIAS_REQ(int slot, string alias)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(slot);
		msgBody.Write(alias);
		Say(54, msgBody);
	}

	public void SendCS_LOAD_COMPLETE_REQ(int crc)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(crc);
		Say(42, msgBody);
	}

	public void SendCS_TEAM_CHAT_REQ(string text)
	{
		if (MyInfoManager.Instance.Slot < 0)
		{
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				string text2 = StringMgr.Instance.Get("WARNING_TEAM_CHAT");
				gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, string.Empty, text2));
			}
		}
		else
		{
			MsgBody msgBody = new MsgBody();
			msgBody.Write(text);
			Say(93, msgBody);
		}
	}

	public void SendCS_CLAN_CHAT_REQ(string text)
	{
		if (MyInfoManager.Instance.ClanSeq < 0)
		{
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				string text2 = StringMgr.Instance.Get("WARNING_CLAN_CHAT");
				gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, string.Empty, text2));
			}
		}
		else
		{
			MsgBody msgBody = new MsgBody();
			msgBody.Write(text);
			Say(94, msgBody);
		}
	}

	public void SendCS_CHAT_REQ(string text)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(text);
		Say(24, msgBody);
	}

	public void SendCS_ROOM_LIST_REQ()
	{
		Say(4, new MsgBody());
	}

	public void SendCS_ROOM_REQ(int no)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(no);
		Say(469, msgBody);
	}

	public void SendCS_KICK_REQ(int seq)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(seq);
		Say(88, msgBody);
	}

	public void SendCS_MAKE_SQUAD_NULL_REQ()
	{
		Say(306, new MsgBody());
	}

	public void SendCS_MAKE_ROOM_NULL_REQ()
	{
		Say(90, new MsgBody());
	}

	public void SendCS_MATCH_COUNTDOWN_REQ(int count)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(count);
		Say(71, msgBody);
	}

	public void SendCS_RANDEZVOUS_POINT_REQ(string localIp, int localPort, string remoteIp, int remotePort)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(localIp);
		msgBody.Write(localPort);
		msgBody.Write(remoteIp);
		msgBody.Write(remotePort);
		Say(83, msgBody);
	}

	public void SendCS_DESTROY_BRICK_REQ(int brick)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(brick);
		Say(76, msgBody);
	}

	public void SendCS_WHISPER_REQ(string listener, string text)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(listener);
		msgBody.Write(text);
		Say(26, msgBody);
	}

	public void SendCS_RESPAWN_TICKET_REQ()
	{
		Say(63, new MsgBody());
	}

	public void SendCS_TEAM_CHANGE_REQ(bool clickSlot, int slotNum)
	{
		if (!_waitingAck)
		{
			MsgBody msgBody = new MsgBody();
			msgBody.Write(clickSlot);
			msgBody.Write(slotNum);
			Say(80, msgBody);
			_waitingAck = true;
		}
	}

	public void SendCS_BREAK_INTO_REQ()
	{
		if (!_waitingAck)
		{
			Say(73, new MsgBody());
			_waitingAck = true;
		}
	}

    //Hooked
	public void SendCS_SAVE_REQ(int slot, byte[] thumbnail)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(slot);
		msgBody.Write(thumbnail.Length);
		for (int i = 0; i < thumbnail.Length; i++)
		{
			msgBody.Write(thumbnail[i]);
		}
		Say(39, msgBody);
	}

	//Hooked
	public void SendCS_REGISTER_REQ(int slot, ushort modeMask, int regHow, int point, int downloadFee, byte[] thumbnail, string msgEval)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(slot);
		msgBody.Write(modeMask);
		msgBody.Write(regHow);
		msgBody.Write(point);
		msgBody.Write(downloadFee);
		msgBody.Write(thumbnail.Length);
		for (int i = 0; i < thumbnail.Length; i++)
		{
			msgBody.Write(thumbnail[i]);
		}
		msgBody.Write(msgEval);
		Say(51, msgBody);
	}

	public void SendCS_CHECK_TERM_ITEM_REQ(long expiring, long alter)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(expiring);
		msgBody.Write(alter);
		Say(123, msgBody);
	}

	public void SendCS_SET_STATUS_REQ(int status)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(status);
		Say(47, msgBody);
	}

	public void SendCS_EQUIP_REQ(long seq)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(seq);
		Say(35, msgBody);
	}

	public void SendCS_UNEQUIP_REQ(long seq)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(seq);
		Say(37, msgBody);
	}

	public void SendCS_RESUME_ROOM_REQ(int nextStatus)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(nextStatus);
		Say(32, msgBody);
	}

	public void SendCS_LEAVE_REQ()
	{
		Say(23, new MsgBody());
	}

	public void SendCS_CACHE_BRICK_REQ()
	{
		Say(20, new MsgBody());
	}

	public void SendCS_SAVE_PALETTE_REQ(int pal0, int pal1, int pal2, int pal3, int pal4, int pal5, int pal6, int pal7, int pal8, int pal9)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(pal0);
		msgBody.Write(pal1);
		msgBody.Write(pal2);
		msgBody.Write(pal3);
		msgBody.Write(pal4);
		msgBody.Write(pal5);
		msgBody.Write(pal6);
		msgBody.Write(pal7);
		msgBody.Write(pal8);
		msgBody.Write(pal9);
		Say(19, msgBody);
	}

	public void SendCS_DEL_BRICK_REQ(int seq)
	{
		if (!_waitingAck)
		{
			MsgBody msgBody = new MsgBody();
			msgBody.Write(seq);
			Say(15, msgBody);
			_waitingAck = true;
		}
	}

	public void SendCS_MORPH_BRICK_REQ(int seq, ushort code)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(seq);
		msgBody.Write(code);
		Say(33, msgBody);
	}

	public void SendCS_ADD_BRICK_REQ(byte brick, byte x, byte y, byte z, byte rot)
	{
		if (!_waitingAck)
		{
			MsgBody msgBody = new MsgBody();
			msgBody.Write(brick);
			msgBody.Write(x);
			msgBody.Write(y);
			msgBody.Write(z);
			msgBody.Write(rot);
			Say(13, msgBody);
			_waitingAck = true;
		}
	}

	public void SendCS_WHATSUP_FELLA_REQ()
	{
		Say(150, new MsgBody());
	}

	public void SendCS_INSTALL_GADGET_REQ(int gadget, float px, float py, float pz, float nx, float ny, float nz)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(gadget);
		msgBody.Write(px);
		msgBody.Write(py);
		msgBody.Write(pz);
		msgBody.Write(nx);
		msgBody.Write(ny);
		msgBody.Write(nz);
		Say(400, msgBody);
	}

	public void SendCS_GADGET_ACTION_REQ(int seq, int action)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(seq);
		msgBody.Write(action);
		Say(402, msgBody);
	}

	public void SendCS_INFLICTED_DAMAGE_REQ(Dictionary<int, int> inflictedDamage)
	{
		MsgBody msgBody = new MsgBody();
		if (inflictedDamage == null)
		{
			msgBody.Write(0);
		}
		else
		{
			msgBody.Write(inflictedDamage.Count);
			foreach (KeyValuePair<int, int> item in inflictedDamage)
			{
				msgBody.Write(item.Key);
				msgBody.Write(item.Value);
			}
		}
		Say(399, msgBody);
	}

	public void SendCS_KILL_LOG_REQ(sbyte killerType, int killer, sbyte victimType, int victim, int weaponBy, int slot, int category, int hitpart, Dictionary<int, int> damageLog)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(killerType);
		msgBody.Write(killer);
		msgBody.Write(victimType);
		msgBody.Write(victim);
		msgBody.Write(weaponBy);
		msgBody.Write(slot);
		msgBody.Write(category);
		msgBody.Write(hitpart);
		if (damageLog == null)
		{
			msgBody.Write(0);
		}
		else
		{
			msgBody.Write(damageLog.Count);
			foreach (KeyValuePair<int, int> item in damageLog)
			{
				msgBody.Write(item.Key);
				msgBody.Write(item.Value);
			}
		}
		Say(44, msgBody);
	}

	public void SendCS_ROAMIN_REQ(int seq, int userType, bool isWebPlayer, int language, string hashCode)
	{
		if (BuildOption.Instance.DontCheckXTrap)
		{
			isWebPlayer = true;
		}
		MsgBody msgBody = new MsgBody();
		msgBody.Write(seq);
		msgBody.Write(userType);
		msgBody.Write(isWebPlayer);
		msgBody.Write(language);
		msgBody.Write(hashCode);
		Say(145, msgBody);
	}

	public void SendCS_ROAMOUT_REQ(int dst)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(dst);
		Say(143, msgBody);
	}

	public void SendCS_INDIVIDUAL_SCORE_REQ()
	{
		Say(178, new MsgBody());
	}

	public void SendCS_LOGIN_REQ(string id, string pswd, int major, int minor)
	{
		if (!_waitingAck)
		{
			MsgBody msgBody = new MsgBody();
			msgBody.Write(id);
			msgBody.Write(pswd);
			msgBody.Write(major);
			msgBody.Write(minor);
			msgBody.Write(privateIpAddress);
			msgBody.Write(macAddress);
			Say(1, msgBody);
            _waitingAck = true;
		}
	}

	public void SendCS_CHARGE_FORCE_POINT_REQ(long item, string code)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(item);
		msgBody.Write(code);
		Say(471, msgBody);
	}

	public void SendCS_CHARGE_PICKNWIN_COIN_REQ(long item, string code)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(item);
		msgBody.Write(code);
		Say(397, msgBody);
	}

	public void SendCS_HASH_CHECK_REQ(string hashCode)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(hashCode);
		Say(395, msgBody);
	}

	public void SendCS_AUTO_LOGIN_REQ(string token, int major, int minor)
	{
		if (!_waitingAck)
		{
			MsgBody msgBody = new MsgBody();
			msgBody.Write(token);
			msgBody.Write(major);
			msgBody.Write(minor);
			msgBody.Write(privateIpAddress);
			msgBody.Write(macAddress);
			Say(393, msgBody);
			_waitingAck = true;
		}
	}

	public void SendCS_AUTO_LOGIN_TO_RUNUP_REQ(string id, string serial, int seq, int major, int minor)
	{
		if (!_waitingAck)
		{
			MsgBody msgBody = new MsgBody();
			msgBody.Write(id);
			msgBody.Write(serial);
			msgBody.Write(seq);
			msgBody.Write(major);
			msgBody.Write(minor);
			msgBody.Write(privateIpAddress);
			msgBody.Write(macAddress);
			Say(409, msgBody);
			_waitingAck = true;
		}
	}

	public void SendCS_TEAM_SCORE_REQ()
	{
		Say(75, new MsgBody());
	}

	public void SendCS_BND_SCORE_REQ()
	{
		Say(324, new MsgBody());
	}

	public void SendCS_ME_CHG_EDITOR_REQ(int seq, bool isEditor)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(seq);
		msgBody.Write(isEditor);
		Say(304, msgBody);
	}

	public bool SendCS_CREATE_ROOM_REQ(int type, string title, bool isLocked, string pswd, int maxPlayer, int[] param, string alias)
	{
		if (_waitingAck)
		{
			return false;
		}
		if (param.Length != 8)
		{
			Debug.LogError("SendCS_CREATE_ROOM_REQ: param should have 8 elements");
			return false;
		}
		BrickManManager.Instance.Clear();
		MsgBody msgBody = new MsgBody();
		msgBody.Write(type);
		msgBody.Write(title);
		msgBody.Write(isLocked);
		msgBody.Write(pswd);
		msgBody.Write(maxPlayer);
		for (int i = 0; i < param.Length; i++)
		{
			msgBody.Write(param[i]);
		}
		msgBody.Write(alias);
		Say(7, msgBody);
		_waitingAck = true;
		return true;
	}

	public void SendCS_ADD_FRIEND_BY_NICKNAME_REQ(string nickname)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(nickname);
		Say(111, msgBody);
	}

	public void SendCS_ADD_BAN_BY_NICKNAME_REQ(string nickname)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(nickname);
		Say(112, msgBody);
	}

	public void SendCS_START_REQ(int remain)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(remain);
		Say(49, msgBody);
	}

	public bool SendCS_QUICK_JOIN_REQ()
	{
		if (_waitingAck)
		{
			return false;
		}
		Say(9, new MsgBody());
		_waitingAck = true;
		return true;
	}

	public bool SendCS_JOIN_REQ(int no, string pswd, bool invite)
	{
		if (_waitingAck)
		{
			return false;
		}
		BrickManManager.Instance.Clear();
		MsgBody msgBody = new MsgBody();
		msgBody.Write(no);
		msgBody.Write(pswd);
		msgBody.Write(invite);
		Say(28, msgBody);
		_waitingAck = true;
		return true;
	}

	private void SendCS_XTRAP_PACKET(byte[] step3Buffer)
	{
		MsgBody msgBody = new MsgBody();
		for (int i = 0; i < step3Buffer.Length; i++)
		{
			msgBody.Write(step3Buffer[i]);
		}
		Say(162, msgBody);
	}

	public void SendCS_HEARTBEAT_REQ(int gmFunction)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(gmFunction);
		Say(3, msgBody);
	}

	public void SendCS_RADIO_MSG_REQ(int category, int msg)
	{
		if (!_waitingAck)
		{
			MsgBody msgBody = new MsgBody();
			msgBody.Write(category);
			msgBody.Write(msg);
			Say(95, msgBody);
			_waitingAck = true;
		}
	}

	public void SendCS_CHANNEL_LIST_REQ()
	{
		Say(140, new MsgBody());
	}

	public void SendCS_LOGOUT_REQ()
	{
		Say(157, new MsgBody());
	}

	public void SendCS_DOWNLOAD_MAP_REQ(int mapSeq, int buyHow)
	{
		if (!_waitingAck)
		{
			MsgBody msgBody = new MsgBody();
			msgBody.Write(mapSeq);
			msgBody.Write(buyHow);
			Say(174, msgBody);
			_waitingAck = true;
		}
	}

	public void SendCS_DEL_DOWNLOAD_MAP_REQ(int mapSeq)
	{
		if (!_waitingAck)
		{
			MsgBody msgBody = new MsgBody();
			msgBody.Write(mapSeq);
			Say(176, msgBody);
			_waitingAck = true;
		}
	}

	public void SendCS_CLAN_MEMBER_REQ(int player, int clan)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(player);
		msgBody.Write(clan);
		Say(233, msgBody);
	}

	public void SendCS_MAP_EVAL_REQ(int map, byte likesOrDislikes, string comment)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(map);
		msgBody.Write(likesOrDislikes);
		msgBody.Write(comment);
		Say(423, msgBody);
	}

	public void SendCS_MY_DOWNLOAD_MAP_REQ(int prevPage, int nextPage, int indexer, ushort modeMask)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(prevPage);
		msgBody.Write(nextPage);
		msgBody.Write(indexer);
		msgBody.Write(modeMask);
		Say(425, msgBody);
	}

	public void SendCS_MY_REGISTER_MAP_REQ(int prevPage, int nextPage, int indexer, ushort modeMask)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(prevPage);
		msgBody.Write(nextPage);
		msgBody.Write(indexer);
		msgBody.Write(modeMask);
		Say(427, msgBody);
	}

	public void SendCS_USER_MAP_REQ(int page)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(page);
		Say(429, msgBody);
	}

	public void SendCS_ALL_MAP_REQ(int prevPage, int nextPage, int indexer, ushort modeMask, int flag, string filter)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(prevPage);
		msgBody.Write(nextPage);
		msgBody.Write(indexer);
		msgBody.Write(modeMask);
		msgBody.Write(flag);
		msgBody.Write(filter);
		Say(431, msgBody);
	}

	public void SendCS_MAP_HONOR_REQ(int prevPage, int nextPage, int indexer)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(prevPage);
		msgBody.Write(nextPage);
		msgBody.Write(indexer);
		Say(435, msgBody);
	}

	public void SendCS_WEEKLY_CHART_REQ(int prevPage, int nextPage, int indexer)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(prevPage);
		msgBody.Write(nextPage);
		msgBody.Write(indexer);
		Say(433, msgBody);
	}

	public void SendCS_MAP_DETAIL_REQ(int mapSeq)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(mapSeq);
		Say(437, msgBody);
	}

	public void SendCS_MORE_COMMENT_REQ(int mapSeq, int lastComment)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(mapSeq);
		msgBody.Write(lastComment);
		Say(439, msgBody);
	}

	public void SendCS_CHG_MAP_INTRO_REQ(int seq, string intro)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(seq);
		msgBody.Write(intro);
		Say(441, msgBody);
	}

	public void SendCS_CHG_MAP_DOWNLOAD_FREE_REQ(int seq, int downloadFee)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(seq);
		msgBody.Write(downloadFee);
		Say(443, msgBody);
	}

	public void SendCS_MAP_DAY_REQ(int prevPage, int nextPage, int indexer)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(prevPage);
		msgBody.Write(nextPage);
		msgBody.Write(indexer);
		Say(445, msgBody);
	}

	public void SendCS_OPEN_DOOR_REQ(int seq)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(seq);
		Say(447, msgBody);
	}

	public void SendCS_CLOSE_DOOR_REQ(int seq)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(seq);
		Say(448, msgBody);
	}

	public void SendCS_AUTO_LOGIN_TO_NETMARBLE_REQ(byte[] CPCookie, int major, int minor)
	{
		if (!_waitingAck)
		{
			MsgBody msgBody = new MsgBody();
			for (int i = 0; i < CPCookie.Length; i++)
			{
				msgBody.Write(CPCookie[i]);
			}
			msgBody.Write(major);
			msgBody.Write(minor);
			msgBody.Write(privateIpAddress);
			msgBody.Write(macAddress);
			Say(458, msgBody);
			_waitingAck = true;
		}
	}

	private void ReceiveCallback(IAsyncResult ar)
	{
		if (_sock != null)
		{
			try
			{
				int num = _sock.EndReceive(ar);
				if (num > 0)
				{
					_recv.Io += num;
					for (Msg4Recv.MsgStatus status = _recv.GetStatus(recvKey); status == Msg4Recv.MsgStatus.COMPLETE; status = _recv.GetStatus(recvKey))
					{
						MsgBody msgBody = _recv.Flush();
						msgBody.Decrypt(recvKey);
						lock (this)
						{
							_readQueue.Enqueue(new Msg2Handle(_recv.GetId(), msgBody));
						}
					}
					_sock.BeginReceive(_recv.Buffer, _recv.Io, _recv.Buffer.Length - _recv.Io, SocketFlags.None, ReceiveCallback, null);
				}
			}
			catch (SocketException ex)
			{
				Debug.LogError("Error, " + ex.Message.ToString());
				Close();
			}
		}
	}

	private void HandleCS_REPLACE_BRICK_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out int val3);
		msg.Read(out byte val4);
		msg.Read(out byte val5);
		msg.Read(out byte val6);
		msg.Read(out byte val7);
		msg.Read(out byte val8);
		MyInfoManager.Instance.IsModified = true;
		BrickManager.Instance.DelBrick(val2, shrink: true);
		Brick brick = BrickManager.Instance.GetBrick(val4);
		if (brick != null)
		{
			BrickManager.Instance.AddBrickCreator(val3, val4, new Vector3((float)(int)val5, (float)(int)val6, (float)(int)val7), val8);
		}
		if (val == MyInfoManager.Instance.Seq)
		{
			((ReplaceToolDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.REPLACE_TOOL))?.MoveNext(success: true);
		}
	}

	private void HandleCS_LINE_BRICK_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out byte val3);
		msg.Read(out byte val4);
		msg.Read(out byte val5);
		msg.Read(out byte val6);
		msg.Read(out byte val7);
		MyInfoManager.Instance.IsModified = true;
		Brick brick = BrickManager.Instance.GetBrick(val3);
		if (brick != null)
		{
			BrickManager.Instance.AddBrickCreator(val2, val3, new Vector3((float)(int)val4, (float)(int)val5, (float)(int)val6), val7);
		}
		if (val == MyInfoManager.Instance.Seq)
		{
			((LineToolDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.LINE_TOOL))?.MoveNext(success: true);
		}
	}

	private void HandleCS_LINE_BRICK_FAIL_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		ShowBuildErrorMessage(val);
		((LineToolDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.LINE_TOOL))?.MoveNext(success: false);
	}

	private void HandleCS_REPLACE_BRICK_FAIL_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		ShowBuildErrorMessage(val);
		((ReplaceToolDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.REPLACE_TOOL))?.MoveNext(success: false);
	}

	private void HandleCS_UPDATE_SCRIPT_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out string val2);
		msg.Read(out bool val3);
		msg.Read(out bool val4);
		msg.Read(out string val5);
		BrickManager.Instance.UpdateScript(val, val2, val3, val4, val5);
	}

	private void HandleCS_RADIO_MSG_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out int val3);
		string text = string.Empty;
		if (val == MyInfoManager.Instance.Seq)
		{
			_waitingAck = false;
			text = MyInfoManager.Instance.Nickname;
		}
		else
		{
			BrickManDesc desc = BrickManManager.Instance.GetDesc(val);
			if (desc != null)
			{
				text = desc.Nickname;
			}
		}
		if (text.Length > 0)
		{
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				RadioMenu component = gameObject.GetComponent<RadioMenu>();
				if (null != component)
				{
					RadioSignal radioSignal = new RadioSignal(val, val2, val3);
					string @string = component.GetString(radioSignal);
					if (@string.Length > 0)
					{
						gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.TEAM, val, text, @string));
						gameObject.BroadcastMessage("OnRadioMsg", radioSignal);
					}
				}
			}
		}
	}

	private void HandleCS_SVC_ENTER_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out string val2);
		msg.Read(out int val3);
		msg.Read(out int val4);
		ChannelUserManager.Instance.AddUser(val, val2, XpManager.Instance.GetLevel(val3), val4);
	}

	private void HandleCS_SVC_LEAVE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		ChannelUserManager.Instance.DelUser(val);
	}

	private void HandleCS_UPDATE_SCRIPT_FAIL_ACK(MsgBody msg)
	{
		MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("FAIL_TO_UPDATE_SCRIPT"));
	}

	private void HandleCS_REGISTER_ACK(MsgBody msg)
	{
		msg.Read(out byte val);
		msg.Read(out int val2);
		if (val2 < 0)
		{
			if (val2 == -14)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("REG_NO_MODE"));
			}
			else
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("FAIL_TO_REGISTER"));
			}
		}
		else
		{
			UserMapInfo userMapInfo = UserMapInfoManager.Instance.Get(val);
			if (userMapInfo != null)
			{
				MyInfoManager.Instance.IsModified = false;
				MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("REGISTER_SUCCESS"), userMapInfo.Alias));
			}
		}
	}

	private void HandleCS_ASSET_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out int val3);
		msg.Read(out int val4);
		msg.Read(out int val5);
		MyInfoManager.Instance.Point = val;
		MyInfoManager.Instance.BrickPoint = val2;
		MyInfoManager.Instance.Cash = val3;
		MyInfoManager.Instance.FreeCoin = val4;
		MyInfoManager.Instance.StarDust = val5;
	}

	private void HandleCS_SLOT_LOCK_ACK(MsgBody msg)
	{
		msg.Read(out sbyte val);
		msg.Read(out sbyte val2);
		if (val < RoomManager.Instance.SlotStatus.Length)
		{
			RoomManager.Instance.SlotStatus[val] = ((val2 != 1) ? true : false);
		}
	}

	private void HandleCS_REG_MAP_INFO_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out string val2);
		msg.Read(out string val3);
		msg.Read(out ushort val4);
		msg.Read(out byte val5);
		msg.Read(out byte val6);
		msg.Read(out int val7);
		msg.Read(out sbyte val8);
		msg.Read(out sbyte val9);
		msg.Read(out sbyte val10);
		msg.Read(out sbyte val11);
		msg.Read(out sbyte val12);
		msg.Read(out int val13);
		msg.Read(out int val14);
		msg.Read(out int val15);
		msg.Read(out int val16);
		msg.Read(out int val17);
		msg.Read(out int val18);
		bool clanMatchable = (val5 & Room.clanMatch) != 0;
		bool officialMap = (val5 & Room.official) != 0;
		bool blocked = (val5 & Room.blocked) != 0;
		DateTime regDate = new DateTime(val7, val8, val9, val10, val11, val12);
		RegMapManager.Instance.GetAlways(val, val2, val3, regDate, val4, clanMatchable, officialMap, val16, val17, val18, val13, val14, val15, val6, blocked);
	}

	private void HandleCS_SAVE_ACK(MsgBody msg)
	{
		msg.Read(out byte val);
		msg.Read(out int val2);
		UserMapInfo userMapInfo = UserMapInfoManager.Instance.Get(val);
		if (userMapInfo != null)
		{
			if (val2 == 0)
			{
				userMapInfo.Alias = UserMapInfoManager.Instance.CurMapName;
				MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("SAVE_SUCCESS"), userMapInfo.Alias));
				MyInfoManager.Instance.IsModified = false;
			}
			else
			{
				MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("SAVE_FAIL"), userMapInfo.Alias));
			}
		}
	}

	private void HandleCS_ROOM_CONFIG_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out string val2);
		msg.Read(out int val3);
		msg.Read(out int val4);
		msg.Read(out int val5);
		msg.Read(out bool val6);
		msg.Read(out bool val7);
		msg.Read(out bool val8);
		msg.Read(out string val9);
		msg.Read(out byte val10);
		msg.Read(out int val11);
		msg.Read(out bool val12);
		msg.Read(out bool val13);
		RoomManager.Instance.SetConfig(val9, val, val2, val3, val4, val5, val6, val7, val8, val10, val12, val13);
		if (RoomManager.Instance.HaveCurrentRoomInfo && RoomManager.Instance.CurrentRoomType != (Room.ROOM_TYPE)val11)
		{
			if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.MISSION || RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BUNGEE)
			{
				for (int i = 0; i < 16; i++)
				{
					RoomManager.Instance.SlotStatus[i] = true;
				}
			}
			if (val11 == 5 || val11 == 7)
			{
				for (int j = 0; j < 16; j++)
				{
					RoomManager.Instance.SlotStatus[j] = (0 <= j && j < 8);
				}
			}
			if (RoomManager.Instance.Master == MyInfoManager.Instance.Seq)
			{
				CSNetManager.Instance.Sock.SendCS_RESUME_ROOM_REQ(0);
			}
			RoomManager.Instance.GetCurrentRoomInfo().Type = (Room.ROOM_TYPE)val11;
		}
	}

	private void HandleCS_BREAK_INTO_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		_waitingAck = false;
		if (val == 0)
		{
			DialogManager.Instance.CloseAll();
			switch (RoomManager.Instance.CurrentRoomType)
			{
			case Room.ROOM_TYPE.TEAM_MATCH:
				Application.LoadLevel("TeamMatch");
				break;
			case Room.ROOM_TYPE.INDIVIDUAL:
				Application.LoadLevel("IndividualMatch");
				break;
			case Room.ROOM_TYPE.CAPTURE_THE_FLAG:
				Application.LoadLevel("CaptureTheFlagMatch");
				break;
			case Room.ROOM_TYPE.EXPLOSION:
				Application.LoadLevel("ExplosionMatch");
				break;
			case Room.ROOM_TYPE.BND:
				Application.LoadLevel("BndMatch");
				break;
			case Room.ROOM_TYPE.BUNGEE:
				Application.LoadLevel("BungeeMatch");
				break;
			case Room.ROOM_TYPE.MISSION:
				Application.LoadLevel("Defense");
				break;
			case Room.ROOM_TYPE.ESCAPE:
				Application.LoadLevel("Escape");
				break;
			case Room.ROOM_TYPE.ZOMBIE:
				Application.LoadLevel("Zombie");
				break;
			default:
				Debug.LogError("Cant start on this room " + RoomManager.Instance.CurrentRoomType);
				break;
			}
		}
		else
		{
			switch (val)
			{
			case -1:
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CANT_BREAK_INTO"));
				break;
			case -2:
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NOT_PLAYING"));
				break;
			}
		}
	}

	private void HandleCS_START_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		if (val == 0)
		{
			DialogManager.Instance.CloseAll();
			if (!Application.isLoadingLevel)
			{
				switch (RoomManager.Instance.CurrentRoomType)
				{
				case Room.ROOM_TYPE.TEAM_MATCH:
					BrickManManager.Instance.ResetGameStuff();
					Application.LoadLevel("TeamMatch");
					break;
				case Room.ROOM_TYPE.MISSION:
					BrickManManager.Instance.ResetGameStuff();
					Application.LoadLevel("Defense");
					break;
				case Room.ROOM_TYPE.INDIVIDUAL:
					BrickManManager.Instance.ResetGameStuff();
					Application.LoadLevel("IndividualMatch");
					break;
				case Room.ROOM_TYPE.CAPTURE_THE_FLAG:
					BrickManManager.Instance.ResetGameStuff();
					Application.LoadLevel("CaptureTheFlagMatch");
					break;
				case Room.ROOM_TYPE.EXPLOSION:
					BrickManManager.Instance.ResetGameStuff();
					Application.LoadLevel("ExplosionMatch");
					break;
				case Room.ROOM_TYPE.BND:
					BrickManManager.Instance.ResetGameStuff();
					Application.LoadLevel("BndMatch");
					break;
				case Room.ROOM_TYPE.BUNGEE:
					BrickManManager.Instance.ResetGameStuff();
					Application.LoadLevel("BungeeMatch");
					break;
				case Room.ROOM_TYPE.ESCAPE:
					BrickManManager.Instance.ResetGameStuff();
					Application.LoadLevel("Escape");
					break;
				case Room.ROOM_TYPE.ZOMBIE:
					BrickManManager.Instance.ResetGameStuff();
					Application.LoadLevel("Zombie");
					break;
				default:
					Debug.LogError("Cant start on this room " + RoomManager.Instance.CurrentRoomType);
					break;
				}
			}
		}
		else
		{
			string text = string.Empty;
			switch (val)
			{
			case -1:
				text = StringMgr.Instance.Get("NOT_READY_YET");
				break;
			case -2:
				text = StringMgr.Instance.Get("ONLY_MASTER_START");
				break;
			case -3:
				text = StringMgr.Instance.Get("LOAD_MAP_FAIL");
				break;
			case -4:
				text = StringMgr.Instance.Get("CLAN_MATCH_MIXED");
				break;
			case -5:
				text = StringMgr.Instance.Get("CLAN_MATCH_SAME_CLAN");
				break;
			case -6:
				text = StringMgr.Instance.Get("CLAN_MATCH_TOO_SMALL");
				break;
			case -7:
				text = StringMgr.Instance.Get("CLAN_MATCH_NOT_SAME_NO");
				break;
			case -8:
				text = StringMgr.Instance.Get("CAN_NOT_PLAY_WITH_NO_ENEMY");
				break;
			case -9:
				text = StringMgr.Instance.Get("AT_LEAST_ONE_FOE_READY");
				break;
			case -10:
				text = StringMgr.Instance.Get("NOTICE_BLOCK_MAP");
				break;
			}
			if (text.Length > 0)
			{
				MessageBoxMgr.Instance.AddMessage(text);
			}
		}
	}

	private void HandleCS_MUTE_ACK(MsgBody msg)
	{
		msg.Read(out string val);
		msg.Read(out int val2);
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, string.Empty, string.Format(StringMgr.Instance.Get("GM_MUTE_SUCCESS"), val, val2.ToString())));
		}
	}

	private void HandleCS_YOU_ARE_MUTE_ACK(MsgBody msg)
	{
		msg.Read(out uint val);
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, string.Empty, string.Format(StringMgr.Instance.Get("MUTE_CHAT_FAIL"), (val / 1000u + 1).ToString())));
		}
	}

	private void HandleCS_SLOT_CHANGE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int _);
		msg.Read(out int val3);
		if (val == MyInfoManager.Instance.Seq)
		{
			MyInfoManager.Instance.Slot = (sbyte)val3;
		}
		else
		{
			BrickManDesc desc = BrickManManager.Instance.GetDesc(val);
			if (desc != null)
			{
				desc.Slot = (sbyte)val3;
			}
		}
	}

	private void HandleCS_TEAM_CHANGE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int _);
		msg.Read(out int val3);
		if (val == MyInfoManager.Instance.Seq)
		{
			_waitingAck = false;
			MyInfoManager.Instance.Slot = (sbyte)val3;
			GameObject gameObject = GameObject.Find("TeamFlag");
			if (null != gameObject)
			{
				gameObject.SendMessage("OnChangeTeam", SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			BrickManDesc desc = BrickManManager.Instance.GetDesc(val);
			if (desc != null)
			{
				desc.Slot = (sbyte)val3;
			}
		}
	}

	private void HandleCS_TEAM_CHANGE_FAIL_ACK(MsgBody msg)
	{
		msg.Read(out int _);
		_waitingAck = false;
		MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("TEAM_CHANGE_FAIL"));
	}

	private void HandleCS_BUILDER_SLOT_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out sbyte val2);
		if (val == MyInfoManager.Instance.Seq)
		{
			MyInfoManager.Instance.Slot = val2;
		}
		else
		{
			BrickManDesc desc = BrickManManager.Instance.GetDesc(val);
			if (desc != null)
			{
				desc.Slot = val2;
			}
		}
	}

	private void HandleCS_SLOT_INFO_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out sbyte val2);
		msg.Read(out int val3);
		msg.Read(out int val4);
		msg.Read(out int val5);
		msg.Read(out int val6);
		msg.Read(out int val7);
		msg.Read(out int val8);
		if (val == MyInfoManager.Instance.Seq)
		{
			MyInfoManager.Instance.Slot = val2;
			MyInfoManager.Instance.Status = val3;
			MyInfoManager.Instance.Kill = val4;
			MyInfoManager.Instance.Death = val5;
			MyInfoManager.Instance.Assist = val6;
			MyInfoManager.Instance.Score = val7;
			MyInfoManager.Instance.Mission = val8;
		}
		else
		{
			BrickManDesc desc = BrickManManager.Instance.GetDesc(val);
			if (desc != null)
			{
				desc.Slot = val2;
				desc.Status = val3;
				desc.Kill = val4;
				desc.Death = val5;
				desc.Assist = val6;
				desc.Score = val7;
				desc.Mission = val8;
			}
		}
	}

	private void HandleCS_EQUIP_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out long val2);
		msg.Read(out string val3);
		val3.Trim();
		if (val == MyInfoManager.Instance.Seq)
		{
			MyInfoManager.Instance.SetItemUsage(val2, val3, Item.USAGE.EQUIP);
			GameObject gameObject = GameObject.Find("BoxMan");
			if (null != gameObject)
			{
				LookCoordinator component = gameObject.GetComponent<LookCoordinator>();
				if (null != component)
				{
					TItem tItem = TItemManager.Instance.Get<TItem>(val3);
					if (tItem == null || tItem.type == TItem.TYPE.CHARACTER || (GlobalVars.Instance.LobbyType != LOBBY_TYPE.SHOP && MyInfoManager.Instance.Status != 5))
					{
						if (val3 == "c17")
						{
							GlobalVars.Instance.SetYangDingVoice(bSet: true);
						}
						component.TestGender = true;
						component.Equip(val3);
						component.TestGender = false;
					}
				}
			}
		}
		else
		{
			BrickManManager.Instance.GetDesc(val)?.Equip(val3);
			GameObject gameObject2 = BrickManManager.Instance.Get(val);
			if (null != gameObject2)
			{
				LookCoordinator component2 = gameObject2.GetComponent<LookCoordinator>();
				if (null != component2)
				{
					component2.Equip(val3);
					component2.ChangeWeapon(RoomManager.Instance.DefaultWeaponType);
				}
			}
		}
	}

	private void HandleCS_UNEQUIP_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out long val2);
		msg.Read(out string val3);
		val3.Trim();
		if (val == MyInfoManager.Instance.Seq)
		{
			MyInfoManager.Instance.SetItemUsage(val2, val3, Item.USAGE.UNEQUIP);
			GameObject gameObject = GameObject.Find("BoxMan");
			if (null != gameObject)
			{
				LookCoordinator component = gameObject.GetComponent<LookCoordinator>();
				if (null != component)
				{
					TItem tItem = TItemManager.Instance.Get<TItem>(val3);
					if (tItem == null || tItem.type == TItem.TYPE.CHARACTER || (GlobalVars.Instance.LobbyType != LOBBY_TYPE.SHOP && MyInfoManager.Instance.Status != 5))
					{
						if (val3 == "c17")
						{
							GlobalVars.Instance.SetYangDingVoice(bSet: false);
						}
						component.Unequip(val3);
					}
				}
			}
		}
		else
		{
			BrickManManager.Instance.GetDesc(val)?.Unequip(val3);
			GameObject gameObject2 = BrickManManager.Instance.Get(val);
			if (null != gameObject2)
			{
				LookCoordinator component2 = gameObject2.GetComponent<LookCoordinator>();
				if (null != component2)
				{
					component2.Unequip(val3);
				}
			}
		}
	}

	private void HandleCS_WHISPER_FAIL_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out string val2);
		string text = string.Empty;
		switch (val)
		{
		case -1:
			text = string.Format(StringMgr.Instance.Get("WHISPER_FAIL_LOGOUT"), val2);
			break;
		case -2:
			text = string.Format(StringMgr.Instance.Get("WHISPER_FAIL_DENY_ALL"), val2);
			break;
		case -5:
			text = string.Format(StringMgr.Instance.Get("WHISPER_FAIL_DENY_ALL"), val2);
			break;
		}
		if (text.Length > 0)
		{
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, string.Empty, text));
			}
		}
	}

	private void HandleCS_TREASURE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out string val2);
		msg.Read(out string val3);
		if (BuildOption.Instance.Props.randomBox == BuildOption.RANDOM_BOX_TYPE.NETMARBLE && val == MyInfoManager.Instance.Seq)
		{
			((TCResultItemDialog)DialogManager.Instance.GetDialogAlways(DialogManager.DIALOG_INDEX.TCRESULT))?.SetRareText(val, val2, val3);
		}
		else
		{
			TItem tItem = TItemManager.Instance.Get<TItem>(val3);
			if (tItem != null)
			{
				string text = string.Format(StringMgr.Instance.Get("TREASURE_GET_CONGRATULATIONS"), val2, tItem.Name);
				GameObject gameObject = GameObject.Find("Main");
				if (null != gameObject)
				{
					gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.TREASURE, val, val2, text));
				}
			}
		}
	}

	private void HandleCS_AUTO_LOGIN_TO_NETMARBLE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out bool val3);
		msg.Read(out int val4);
		msg.Read(out int val5);
		msg.Read(out byte val6);
		msg.Read(out int val7);
		_waitingAck = false;
		if (val >= 0)
		{
			MyInfoManager.Instance.Seq = val;
			MyInfoManager.Instance.PlayAuthCode = val5;
			ChannelManager.Instance.LoginChannelId = val2;
			MyInfoManager.Instance.NeedPlayerInfo = val3;
			BuffManager.Instance.netCafeCode = val4;
			MyInfoManager.Instance.SiteCode = val6;
			MyInfoManager.Instance.Age = val7;
			if (!val3)
			{
				if (BuildOption.Instance.Props.ShowAgb && !MyInfoManager.Instance.AgreeTos)
				{
					Application.LoadLevel("Tos");
				}
				else
				{
					Application.LoadLevel("BfStart");
				}
			}
			else
			{
				MyInfoManager.Instance.AgreeTos = false;
				if (BuildOption.Instance.Props.ShowAgb && !MyInfoManager.Instance.AgreeTos)
				{
					Application.LoadLevel("Tos");
				}
				else
				{
					Application.LoadLevel("PlayerInfo");
				}
			}
		}
		else
		{
			string text = string.Empty;
			switch (val)
			{
			case -112:
				text = StringMgr.Instance.Get("WARNING_SYS_ERROR");
				break;
			case -113:
				text = StringMgr.Instance.Get("WARNING_TIMEOUT");
				break;
			case -114:
				switch (val5)
				{
				case 1:
					text = StringMgr.Instance.Get("COOLING_OFF_TOTAL_OVER");
					break;
				case 2:
					text = StringMgr.Instance.Get("COOLING_OFF_COUTINOUS_OVER");
					break;
				case 3:
					text = StringMgr.Instance.Get("PARENTS_ALLOW_OVER");
					break;
				case 4:
					text = StringMgr.Instance.Get("SHUTDOWN_WARNING_02");
					break;
				}
				break;
			default:
				text = GetLoginFailString(val);
				break;
			}
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject && text.Length > 0)
			{
				gameObject.BroadcastMessage("OnLoginFailMessage", text);
			}
		}
	}

	private void HandleCS_CHAT_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out byte val2);
		msg.Read(out string val3);
		msg.Read(out string val4);
		msg.Read(out bool val5);
		if (!MyInfoManager.Instance.IsBan(val))
		{
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				gameObject.BroadcastMessage("OnChat", new ChatText((ChatText.CHAT_TYPE)val2, val, val3, val4, val5));
			}
		}
	}

	private void HandleCS_PLAYER_INFO_ACK(MsgBody msg)
	{
		msg.Read(out string val);
		msg.Read(out int val2);
		msg.Read(out int val3);
		msg.Read(out int val4);
		msg.Read(out int val5);
		msg.Read(out int _);
		msg.Read(out int val7);
		msg.Read(out int val8);
		msg.Read(out int val9);
		msg.Read(out int val10);
		msg.Read(out int val11);
		msg.Read(out int val12);
		msg.Read(out string val13);
		msg.Read(out int val14);
		msg.Read(out int val15);
		msg.Read(out int val16);
		msg.Read(out int val17);
		msg.Read(out int val18);
		msg.Read(out int val19);
		msg.Read(out int val20);
		msg.Read(out int val21);
		msg.Read(out int val22);
		msg.Read(out int val23);
		MyInfoManager.Instance.Nickname = val;
		MyInfoManager.Instance.Xp = val2;
		MyInfoManager.Instance.Point = val3;
		MyInfoManager.Instance.BrickPoint = val4;
		MyInfoManager.Instance.Cash = val5;
		MyInfoManager.Instance.FreeCoin = val7;
		MyInfoManager.Instance.StarDust = val8;
		MyInfoManager.Instance.GM = val11;
		NoCheat.Instance.Sync(NoCheat.WATCH_DOG.GM, MyInfoManager.Instance.GM);
		NoCheat.Instance.Sync(NoCheat.WATCH_DOG.GODMODE, MyInfoManager.Instance.GodMode ? 1 : 0);
		NoCheat.Instance.Sync(NoCheat.WATCH_DOG.GHOSTMODE, MyInfoManager.Instance.isGhostOn ? 1 : 0);
		MyInfoManager.Instance.ClanSeq = val12;
		MyInfoManager.Instance.ClanName = val13;
		MyInfoManager.Instance.ClanMark = val14;
		MyInfoManager.Instance.ClanLv = val15;
		MyInfoManager.Instance.Rank = val16;
		MyInfoManager.Instance.Heavy = val17;
		MyInfoManager.Instance.Assault = val18;
		MyInfoManager.Instance.Sniper = val19;
		MyInfoManager.Instance.SubMachine = val20;
		MyInfoManager.Instance.HandGun = val21;
		MyInfoManager.Instance.Melee = val22;
		MyInfoManager.Instance.Special = val23;
		//XTrap.Instance.SetUserInfo(val);
		Aps.Instance.SetLevel(val9, val10);
	}

	private void HandleCS_CACHE_BRICK_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		for (int i = 0; i < val; i++)
		{
			msg.Read(out int val2);
			msg.Read(out byte val3);
			msg.Read(out byte val4);
			msg.Read(out byte val5);
			msg.Read(out byte val6);
			msg.Read(out ushort val7);
			msg.Read(out byte val8);
			msg.Read(out byte val9);
			BrickManager.Instance.CacheBrick(val2, val3, val4, val5, val6, val7, val8);
			if (val9 > 0)
			{
				msg.Read(out string val10);
				msg.Read(out bool val11);
				msg.Read(out bool val12);
				msg.Read(out string val13);
				BrickManager.Instance.UpdateScript(val2, val10, val11, val12, val13);
			}
		}
	}

	private void HandleCS_CACHE_BRICK_DONE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		BrickManager.Instance.CacheDone(val, val2);
	}

	private void HandleCS_EDIT_BRICK_FAIL_ACK(MsgBody msg)
	{
		_waitingAck = false;
		msg.Read(out int val);
		ShowBuildErrorMessage(val);
	}

	private void ShowBuildErrorMessage(int result)
	{
		switch (result)
		{
		case -2:
			SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("OVER_NUM"));
			break;
		case -3:
			SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("NO_MAPEDIT_AUTH"));
			break;
		case -4:
			SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get(string.Empty));
			break;
		case -5:
			SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get(string.Empty));
			break;
		case -6:
			SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("BRICK_REMOVE_WARNING_MSG"));
			break;
		}
	}

	private void HandleCS_DESTROYED_BRICK_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		BrickManager.Instance.DelBrick(val, shrink: false);
	}

	private void HandleCS_DESTROY_BRICK_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		BrickManager.Instance.DestroyBrick(val);
	}

	private void HandleCS_DEL_BRICK_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		AudioSource audioSource = null;
		GameObject gameObject = GameObject.Find("Me");
		LocalController localController = null;
		if (null != gameObject)
		{
			localController = gameObject.GetComponent<LocalController>();
		}
		if (MyInfoManager.Instance.Seq == val)
		{
			_waitingAck = false;
			if (null != localController)
			{
				audioSource = localController.GetComponent<AudioSource>();
			}
			if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BUNGEE)
			{
				GameObject[] array = BrickManManager.Instance.ToGameObjectArray();
				if (array != null)
				{
					for (int i = 0; i < array.Length; i++)
					{
						TPController component = array[i].GetComponent<TPController>();
						GameObject brickObject = BrickManager.Instance.GetBrickObject(val2);
						if (brickObject != null && component != null)
						{
							float num = Vector3.Distance(brickObject.transform.position, component.transform.position);
							if (num < 3f)
							{
								UnityEngine.Object.Instantiate((UnityEngine.Object)BrickManager.Instance.bungeeFeedbackEffects, brickObject.transform.position, brickObject.transform.rotation);
								GameObject gameObject2 = GameObject.Find("Main");
								if (null != gameObject2)
								{
									BungeeMatch component2 = gameObject2.GetComponent<BungeeMatch>();
									if (component2 != null)
									{
										component2.OnEffectivePoint(brickObject.transform.position, num);
									}
								}
								break;
							}
						}
					}
				}
			}
			if (GlobalVars.Instance.IsFeverMode())
			{
				GlobalVars.Instance.addFeverCounter();
			}
		}
		else
		{
			GameObject gameObject3 = BrickManManager.Instance.Get(val);
			if (gameObject3 != null)
			{
				audioSource = gameObject3.GetComponent<AudioSource>();
			}
			if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BUNGEE)
			{
				GameObject brickObject2 = BrickManager.Instance.GetBrickObject(val2);
				if (brickObject2 != null && localController != null)
				{
					float num2 = Vector3.Distance(brickObject2.transform.position, localController.TranceformPosition);
					int num3 = 0;
					if (num2 < 1f)
					{
						num3 = 3;
					}
					else if (num2 < 2f)
					{
						num3 = 2;
					}
					else if (num2 < 3f)
					{
						num3 = 1;
					}
					if (num3 > 0)
					{
						Vector3 position = brickObject2.transform.position;
						float y = position.y;
						Vector3 tranceformPosition = localController.TranceformPosition;
						if (y <= tranceformPosition.y)
						{
							num3++;
						}
					}
					localController.LogAttacker(val, num3);
				}
			}
		}
		if (BuildOption.Instance.Props.brickSoundChange && BrickManager.Instance.userMap != null)
		{
			BrickInst brickInst = BrickManager.Instance.userMap.Get(val2);
			if (brickInst != null)
			{
				Brick brick = BrickManager.Instance.GetBrick(brickInst.Template);
				if (brick != null && audioSource != null && brick.delSound != null)
				{
					audioSource.PlayOneShot(brick.delSound);
				}
			}
		}
		MyInfoManager.Instance.IsModified = true;
		BrickManager.Instance.DelBrick(val2, shrink: true);
	}

	private void HandleCS_ADD_BRICK_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out byte val3);
		msg.Read(out byte val4);
		msg.Read(out byte val5);
		msg.Read(out byte val6);
		msg.Read(out byte val7);
		AudioSource audioSource = null;
		if (MyInfoManager.Instance.Seq == val)
		{
			_waitingAck = false;
			GameObject gameObject = GameObject.Find("Me");
			if (null != gameObject)
			{
				LocalController component = gameObject.GetComponent<LocalController>();
				if (null != component)
				{
					audioSource = component.GetComponent<AudioSource>();
				}
			}
			if (GlobalVars.Instance.IsFeverMode())
			{
				GlobalVars.Instance.addFeverCounter();
			}
		}
		else
		{
			GameObject gameObject2 = BrickManManager.Instance.Get(val);
			if (gameObject2 != null)
			{
				audioSource = gameObject2.GetComponent<AudioSource>();
			}
		}
		MyInfoManager.Instance.IsModified = true;
		Brick brick = BrickManager.Instance.GetBrick(val3);
		if (brick != null)
		{
			if (BuildOption.Instance.Props.brickSoundChange && audioSource != null && brick.addSound != null)
			{
				audioSource.PlayOneShot(brick.addSound);
			}
			BrickManager.Instance.AddBrickCreator(val2, val3, new Vector3((float)(int)val4, (float)(int)val5, (float)(int)val6), val7);
		}
	}

	private void HandleCS_ADD_ROOM_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out string val3);
		msg.Read(out bool val4);
		msg.Read(out int val5);
		msg.Read(out int val6);
		msg.Read(out int val7);
		msg.Read(out int val8);
		msg.Read(out string val9);
		msg.Read(out int val10);
		msg.Read(out int val11);
		msg.Read(out int val12);
		msg.Read(out int val13);
		msg.Read(out int val14);
		msg.Read(out int val15);
		msg.Read(out int val16);
		msg.Read(out bool val17);
		msg.Read(out bool val18);
		msg.Read(out bool val19);
		msg.Read(out int val20);
		msg.Read(out int val21);
		RoomManager.Instance.AddOrUpdateRoom(val, (Room.ROOM_STATUS)val5, val6, val7, val4, val8, val9, val10, val11, val12, val13, val14, val15, val16, val17, val2, val3, val18, val19, val20, val21);
	}

	private void HandleCS_MASTER_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		RoomManager.Instance.Master = val;
		if (Application.loadedLevelName.Contains("CaptureTheFlagMatch"))
		{
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				gameObject.BroadcastMessage("OnChangeRoomMaster");
			}
		}
		if (MyInfoManager.Instance.Seq == RoomManager.Instance.Master && Application.loadedLevelName.Contains("Defense"))
		{
			MonManager.Instance.AiReset();
		}
	}

	private void HandleCS_UPDATE_ROOM_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out int val3);
		msg.Read(out int val4);
		msg.Read(out bool val5);
		msg.Read(out int val6);
		msg.Read(out string val7);
		msg.Read(out int val8);
		msg.Read(out int val9);
		msg.Read(out int val10);
		msg.Read(out int val11);
		msg.Read(out int val12);
		msg.Read(out int val13);
		msg.Read(out int val14);
		msg.Read(out bool val15);
		msg.Read(out int val16);
		msg.Read(out string val17);
		msg.Read(out bool val18);
		msg.Read(out bool val19);
		msg.Read(out int val20);
		msg.Read(out int val21);
		RoomManager.Instance.AddOrUpdateRoom(val, (Room.ROOM_STATUS)val2, val3, val4, val5, val6, val7, val8, val9, val10, val11, val12, val13, val14, val15, val16, val17, val18, val19, val20, val21);
	}

	private void HandleCS_DEL_ROOM_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		RoomManager.Instance.DelRoom(val);
	}

	private void HandleCS_PALETTE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out int val3);
		msg.Read(out int val4);
		msg.Read(out int val5);
		msg.Read(out int val6);
		msg.Read(out int val7);
		msg.Read(out int val8);
		msg.Read(out int val9);
		msg.Read(out int val10);
		PaletteManager.Instance.Setup(val, val2, val3, val4, val5, val6, val7, val8, val9, val10);
	}

	private void HandleCS_CREATE_ROOM_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out string val3);
		if (val2 >= 0)
		{
			RoomManager.Instance.CurrentRoom = val2;
			if (val == 0)
			{
				Application.LoadLevel("MapEditor");
			}
			else
			{
				if (ChannelManager.Instance.CurChannel.Mode == 4)
				{
					GlobalVars.Instance.clanTeamMatchSuccess = -1;
				}
				Application.LoadLevel("Briefing4TeamMatch");
			}
		}
		else if (val2 == -32)
		{
			MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("BAD_WORD_DETECT"), val3));
		}
		else
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("FAIL2CREATEROOM"));
		}
		_waitingAck = false;
	}

	private void HandleCS_BACK2BRIEFING_ACK(MsgBody msg)
	{
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.MAP_EDITOR)
		{
			Application.LoadLevel("MapEditor");
		}
		else
		{
			Application.LoadLevel("Briefing4TeamMatch");
		}
	}

	private void HandleCS_JOIN_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		bool flag = true;
		if (Application.loadedLevelName == "SceneSwitch")
		{
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				gameObject.BroadcastMessage("OnJoin", val);
				flag = false;
			}
		}
		if (flag)
		{
			Room room = RoomManager.Instance.GetRoom(val);
			if (room != null)
			{
				RoomManager.Instance.CurrentRoom = val;
				if (room.Type == Room.ROOM_TYPE.MAP_EDITOR)
				{
					Application.LoadLevel("MapEditor");
				}
				else
				{
					Application.LoadLevel("Briefing4TeamMatch");
				}
			}
			else
			{
				string msg2 = StringMgr.Instance.Get("FAIL_TO_JOIN_ROOM");
				switch (val)
				{
				case -2:
					msg2 = StringMgr.Instance.Get("EXCEED");
					break;
				case -3:
					msg2 = StringMgr.Instance.Get("WRONG_PSWD");
					break;
				case -5:
					msg2 = StringMgr.Instance.Get("LOADING_WAIT");
					break;
				case -6:
					msg2 = StringMgr.Instance.Get("CANT_BREAK_INTO");
					break;
				case -8:
					msg2 = StringMgr.Instance.Get("KICKOFF_CUR_ROOM");
					break;
				case -10:
					msg2 = StringMgr.Instance.Get("CLAN_MATCH_SEARCING");
					break;
				}
				MessageBoxMgr.Instance.AddMessage(msg2);
			}
		}
		_waitingAck = false;
	}

	private void HandleCS_QUICK_JOIN_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		if (val < 0)
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("FAIL_TO_QUICK_JOIN_ROOM"));
		}
		else if (ChannelManager.Instance.CurChannelId != val)
		{
			Compass.Instance.SetDestination(Compass.DESTINATION_LEVEL.ROOM, val, val2, string.Empty);
		}
		else
		{
			Room room = RoomManager.Instance.GetRoom(val2);
			if (room != null)
			{
				RoomManager.Instance.CurrentRoom = val2;
				if (room.Type == Room.ROOM_TYPE.MAP_EDITOR)
				{
					Application.LoadLevel("MapEditor");
				}
				else
				{
					Application.LoadLevel("Briefing4TeamMatch");
				}
			}
			else
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("FAIL_TO_QUICK_JOIN_ROOM"));
			}
		}
		_waitingAck = false;
	}

	private void HandleCS_LOAD_START_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		if (val == MyInfoManager.Instance.Seq)
		{
			MyInfoManager.Instance.Status = 2;
		}
		else
		{
			BrickManDesc desc = BrickManManager.Instance.GetDesc(val);
			if (desc != null)
			{
				desc.Status = 2;
			}
		}
		GameObject gameObject = GameObject.Find("Main");
		if (gameObject != null)
		{
			MapEditor component = gameObject.GetComponent<MapEditor>();
			if (component != null)
			{
				component.AddLoadPlayer(val);
			}
		}
	}

	private void HandleCS_TUTORIAL_COMPLETE_ACK(MsgBody msg)
	{
		msg.Read(out bool val);
		msg.Read(out sbyte val2);
		if (val && (!GlobalVars.Instance.isLoadBattleTutor || MyInfoManager.Instance.Tutorialed != 1) && (GlobalVars.Instance.isLoadBattleTutor || MyInfoManager.Instance.Tutorialed != 2))
		{
			MyInfoManager instance = MyInfoManager.Instance;
			instance.Tutorialed = (sbyte)(instance.Tutorialed + val2);
			if (MyInfoManager.Instance.Tutorialed >= 3)
			{
				MyInfoManager.Instance.Tutorialed = 3;
			}
			if (val2 == 1 || val2 == 2)
			{
				((TutorCompleteDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.TUTOR_COMPLETE, exclusive: true))?.InitDialog();
			}
		}
	}

	private void HandleCS_APS_WARNING_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		string text = Aps.Instance.SetLevel(val, val2);
		if (text.Length > 0)
		{
			SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get(text));
		}
	}

	private void HandleCS_LOAD_COMPLETE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		if (val == MyInfoManager.Instance.Seq)
		{
			MyInfoManager.Instance.Status = 3;
			P2PManager.Instance.StartSession();
		}
		else
		{
			BrickManDesc desc = BrickManManager.Instance.GetDesc(val);
			if (desc != null)
			{
				desc.Status = 3;
			}
		}
	}

	private void HandleCS_RENDEZVOUS_POINT_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out string val2);
		msg.Read(out int val3);
		msg.Read(out string val4);
		msg.Read(out int val5);
		P2PManager.Instance.Refresh(val, val2, val3, val4, val5);
	}

	private void HandleCS_ENTER_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out string val2);
		msg.Read(out string val3);
		msg.Read(out int val4);
		msg.Read(out string val5);
		msg.Read(out int val6);
		msg.Read(out int val7);
		string[] array = new string[val7];
		for (int i = 0; i < val7; i++)
		{
			msg.Read(out array[i]);
		}
		msg.Read(out int val8);
		msg.Read(out int val9);
		msg.Read(out int val10);
		msg.Read(out string val11);
		msg.Read(out int val12);
		msg.Read(out int val13);
		msg.Read(out byte val14);
		msg.Read(out val7);
		string[] array2 = (val7 > 0) ? new string[val7] : null;
		for (int j = 0; j < val7; j++)
		{
			msg.Read(out array2[j]);
		}
		msg.Read(out val7);
		string[] array3 = (val7 > 0) ? new string[val7] : null;
		for (int k = 0; k < val7; k++)
		{
			msg.Read(out array3[k]);
		}
		P2PManager.Instance.Add(val, val3, val4, val5, val6, val14);
		BrickManManager.Instance.OnEnter(val, val2, array, val8, val9, val10, val11, val12, val13, array2, array3);
		if (RoomManager.Instance.CurrentRoom >= 0)
		{
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, val, val2, StringMgr.Instance.Get("ENTERED")));
			}
		}
	}

	private void HandleCS_LEAVE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		Room.ROOM_TYPE currentRoomType = RoomManager.Instance.CurrentRoomType;
		if (currentRoomType == Room.ROOM_TYPE.CAPTURE_THE_FLAG && val == BrickManManager.Instance.haveFlagSeq)
		{
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				gameObject.BroadcastMessage("OnLeaved", val);
			}
		}
		BrickManDesc desc = BrickManManager.Instance.GetDesc(val);
		GameObject gameObject2 = GameObject.Find("Main");
		if (null != gameObject2 && desc != null)
		{
			gameObject2.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, val, desc.Nickname, StringMgr.Instance.Get("LEFT")));
		}
		P2PManager.Instance.Remove(val);
		BrickManManager.Instance.Remove(val);
		if (gameObject2 != null)
		{
			MapEditor component = gameObject2.GetComponent<MapEditor>();
			if (component != null)
			{
				component.RemoveLoadPlayer(val);
			}
		}
		_waitingAck = false;
	}

	private void HandleCS_I_AGREE_TOS_ACK(MsgBody msg)
	{
		Application.LoadLevel("BfStart");
	}

	private void HandleCS_AUTO_LOGIN_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		_waitingAck = false;
		if (val >= 0)
		{
			MyInfoManager.Instance.Seq = val;
			ChannelManager.Instance.LoginChannelId = val2;
			if (!BuildOption.Instance.Props.ShowAgb || MyInfoManager.Instance.AgreeTos)
			{
				Application.LoadLevel("BfStart");
			}
			else
			{
				Application.LoadLevel("Tos");
			}
		}
		else
		{
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				gameObject.BroadcastMessage("OnLoginFailMessage", GetLoginFailString(val));
			}
		}
	}

	private void HandleCS_AUTO_LOGIN_TO_RUNUP_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out bool val3);
		_waitingAck = false;
		if (val >= 0)
		{
			MyInfoManager.Instance.Seq = val;
			ChannelManager.Instance.LoginChannelId = val2;
			MyInfoManager.Instance.NeedPlayerInfo = val3;
			if (!val3)
			{
				if (BuildOption.Instance.Props.ShowAgb && !MyInfoManager.Instance.AgreeTos)
				{
					Application.LoadLevel("Tos");
				}
				else
				{
					Application.LoadLevel("BfStart");
				}
			}
			else
			{
				MyInfoManager.Instance.AgreeTos = false;
				if (BuildOption.Instance.Props.ShowAgb && !MyInfoManager.Instance.AgreeTos)
				{
					Application.LoadLevel("Tos");
				}
				else
				{
					Application.LoadLevel("PlayerInfo");
				}
			}
		}
		else
		{
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				gameObject.BroadcastMessage("OnLoginFailMessage", GetLoginFailString(val));
			}
		}
	}

	private void HandleCS_CHARGE_PICKNWIN_COIN_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out long _);
		msg.Read(out string val3);
		msg.Read(out int val4);
		if (val >= 0)
		{
			TItem tItem = TItemManager.Instance.Get<TItem>(val3);
			if (tItem != null)
			{
				string msg2 = string.Format(StringMgr.Instance.Get("PICKNWIN_COIN_CHARGED"), tItem.Name, val4);
				SystemMsgManager.Instance.ShowMessage(msg2);
			}
		}
	}

	private void HandleCS_CHARGE_FORCE_POINT_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out long _);
		msg.Read(out string val3);
		msg.Read(out int val4);
		if (val >= 0)
		{
			TItem tItem = TItemManager.Instance.Get<TItem>(val3);
			if (tItem != null)
			{
				string msg2 = string.Format(StringMgr.Instance.Get("FORCE_POINT_CHARGED"), tItem.Name, val4);
				SystemMsgManager.Instance.ShowMessage(msg2);
			}
		}
	}

	private void HandleCS_GADGET_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int _);
		msg.Read(out int _);
		msg.Read(out int val4);
		msg.Read(out float val5);
		msg.Read(out float val6);
		msg.Read(out float val7);
		msg.Read(out float val8);
		msg.Read(out float val9);
		msg.Read(out float val10);
		if (MyInfoManager.Instance.Seq == val4)
		{
			MyInfoManager.Instance.SenseBombSeq = val;
		}
		else
		{
			GameObject gameObject = BrickManManager.Instance.Get(val4);
			if (null != gameObject)
			{
				TPController component = gameObject.GetComponent<TPController>();
				if (null != component)
				{
					Vector3 pos = new Vector3(val5, val6, val7);
					Vector3 normal = new Vector3(val8, val9, val10);
					BrickManDesc desc = BrickManManager.Instance.GetDesc(val4);
					component.SetSenseBeam(desc.Slot, val, pos, normal);
				}
			}
		}
	}

	private void HandleCS_GADGET_ACTION_ACK(MsgBody msg)
	{
		msg.Read(out int _);
		msg.Read(out int _);
	}

	private void HandleCS_GADGET_REMOVE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		GameObject gameObject = GameObject.Find("Me");
		if (null != gameObject)
		{
			EquipCoordinator component = gameObject.GetComponent<EquipCoordinator>();
			if (null != component)
			{
				component.SelfKaboomSenseBomb(val);
			}
		}
		GameObject[] array = BrickManManager.Instance.ToGameObjectArray();
		for (int i = 0; i < array.Length; i++)
		{
			TPController component2 = array[i].GetComponent<TPController>();
			if (null != component2)
			{
				component2.KaboomSenseBoom(val);
			}
		}
	}

	private void HandleCS_HASH_CHECK_ERROR_ACK(MsgBody msg)
	{
		NoCheat.Instance.AutoRepair();
	}

	private void HandleCS_LOGIN_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
        _waitingAck = false;
		if (val >= 0)
		{
			if (GlobalVars.Instance.bRemember)
			{
				PlayerPrefs.SetString("myID", GlobalVars.Instance.strMyID);
			}
			else
			{
				PlayerPrefs.SetString("myID", string.Empty);
			}
			MyInfoManager.Instance.Seq = val;
			ChannelManager.Instance.LoginChannelId = val2;
			if (!BuildOption.Instance.Props.ShowAgb || MyInfoManager.Instance.AgreeTos)
			{
				Application.LoadLevel("BfStart");
			}
			else
			{
				Application.LoadLevel("Tos");
			}
		}
		else
		{
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				gameObject.BroadcastMessage("OnLoginFailMessage", GetLoginFailString(val));
			}
		}
	}

	private void HandleCS_USERMAP_ACK(MsgBody msg)
	{
		msg.Read(out byte val);
		msg.Read(out string val2);
		msg.Read(out int val3);
		msg.Read(out int val4);
		msg.Read(out sbyte val5);
		msg.Read(out sbyte val6);
		msg.Read(out sbyte val7);
		msg.Read(out sbyte val8);
		msg.Read(out sbyte val9);
		msg.Read(out sbyte val10);
		DateTime lastModified = DateTime.MinValue;
		if (val2.Length > 0)
		{
			lastModified = ((val4 <= 0) ? new DateTime(1971, 12, 29) : new DateTime(val4, val5, val6, val7, val8, val9));
		}
		UserMapInfoManager.Instance.AddOrUpdate(val, val2, val3, lastModified, val10);
	}

	private void HandleCS_USERMAP_LIST_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		for (int i = 0; i < val; i++)
		{
			msg.Read(out byte val2);
			msg.Read(out string val3);
			msg.Read(out int val4);
			msg.Read(out int val5);
			msg.Read(out sbyte val6);
			msg.Read(out sbyte val7);
			msg.Read(out sbyte val8);
			msg.Read(out sbyte val9);
			msg.Read(out sbyte val10);
			msg.Read(out sbyte val11);
			DateTime lastModified = DateTime.MinValue;
			if (val3.Length > 0)
			{
				lastModified = ((val5 > 0) ? new DateTime(val5, val6, val7, val8, val9, val10) : new DateTime(1971, 12, 29));
			}
			UserMapInfoManager.Instance.AddOrUpdate(val2, val3, val4, lastModified, val11);
		}
		UserMapInfoManager.Instance.ValidateEmpty();
	}

	private void HandleCS_KILL_LOG_ACK(MsgBody msg)
	{
		msg.Read(out sbyte val);
		msg.Read(out int val2);
		msg.Read(out sbyte val3);
		msg.Read(out int val4);
		msg.Read(out int val5);
		msg.Read(out int val6);
		if (val != 1 && val3 != 1)
		{
			string killerNickname = string.Empty;
			string victimNickname = string.Empty;
			if (val2 == MyInfoManager.Instance.Seq)
			{
				killerNickname = MyInfoManager.Instance.Nickname;
			}
			else
			{
				BrickManDesc desc = BrickManManager.Instance.GetDesc(val2);
				if (desc != null)
				{
					killerNickname = desc.Nickname;
				}
			}
			if (val4 == MyInfoManager.Instance.Seq)
			{
				victimNickname = MyInfoManager.Instance.Nickname;
			}
			else
			{
				BrickManDesc desc2 = BrickManManager.Instance.GetDesc(val4);
				if (desc2 != null)
				{
					victimNickname = desc2.Nickname;
				}
			}
			if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.ZOMBIE)
			{
				if (ZombieVsHumanManager.Instance.IsZombie(val4))
				{
					if (val6 == 4)
					{
						ZombieVsHumanManager.Instance.Die(val4);
					}
				}
				else if (ZombieVsHumanManager.Instance.IsHuman(val4))
				{
					ZombieVsHumanManager.Instance.Die(val4);
				}
			}
			GameObject gameObject = BrickManManager.Instance.Get(val2);
			if (gameObject != null)
			{
				gameObject.SendMessage("OnFeel", FacialExpressor.EXPRESSION.LAUGH);
			}
			GameObject gameObject2 = GameObject.Find("Main");
			if (null != gameObject2)
			{
				Texture2D texture2D = (val5 < 0) ? GlobalVars.Instance.GetWeaponByExcetion(val5) : TItemManager.Instance.GetWeaponBy(val5);
				if (texture2D != null)
				{
					Texture2D headshotImage = null;
					if (val6 == 4)
					{
						headshotImage = TItemManager.Instance.GetWeaponBy(1);
					}
					gameObject2.BroadcastMessage("OnKillLog", new KillInfo(killerNickname, victimNickname, texture2D, headshotImage, val2, val4, val5));
				}
			}
		}
	}

	private void HandleCS_SET_STATUS_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		if (val == MyInfoManager.Instance.Seq)
		{
			MyInfoManager.Instance.Status = val2;
		}
		else
		{
			BrickManDesc desc = BrickManManager.Instance.GetDesc(val);
			if (desc != null)
			{
				desc.Status = val2;
			}
		}
	}

	private void HandleCS_COPYRIGHT_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		UserMapInfoManager.Instance.master = val;
		if (val == MyInfoManager.Instance.Seq)
		{
			UserMapInfoManager.Instance.VerifyCurMapName(val2);
		}
	}

	private void HandleCS_RESPAWN_TICKET_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		MyInfoManager.Instance.Ticket = val;
	}

	private void HandleCS_TIMER_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnTimer", val, SendMessageOptions.DontRequireReceiver);
			gameObject.BroadcastMessage("OnPlayTime", val2, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void HandleCS_DEATH_COUNT_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		if (val == MyInfoManager.Instance.Seq)
		{
			MyInfoManager.Instance.Death = val2;
		}
		else
		{
			BrickManDesc desc = BrickManManager.Instance.GetDesc(val);
			if (desc != null)
			{
				desc.Death = val2;
			}
		}
	}

	private void HandleCS_MISSION_END_ACK(MsgBody msg)
	{
		msg.Read(out sbyte val);
		msg.Read(out sbyte val2);
		msg.Read(out int val3);
		List<ResultUnit> list = new List<ResultUnit>();
		for (int i = 0; i < val3; i++)
		{
			msg.Read(out bool val4);
			msg.Read(out int val5);
			msg.Read(out string val6);
			msg.Read(out int val7);
			msg.Read(out int val8);
			msg.Read(out int val9);
			msg.Read(out int val10);
			msg.Read(out int val11);
			msg.Read(out int val12);
			msg.Read(out int val13);
			msg.Read(out int val14);
			msg.Read(out int val15);
			msg.Read(out long val16);
			list.Add(new ResultUnit(val4, val5, val6, val7, val8, val9, val10, val11, val12, val13, val14, val15, val16));
		}
		list.Sort((ResultUnit prev, ResultUnit next) => prev.Compare(next));
		RoomManager.Instance.RU = list.ToArray();
		RoomManager.Instance.endCode4Red = val;
		RoomManager.Instance.endCode4Blue = val2;
		RoomManager.Instance.redTotalKill = 0;
		RoomManager.Instance.redTotalDeath = 0;
		RoomManager.Instance.blueTotalKill = 0;
		RoomManager.Instance.blueTotalDeath = 0;
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			sbyte b = 0;
			if (MyInfoManager.Instance.Slot < 4)
			{
				if (val == 1)
				{
					b = 1;
				}
				if (val2 == 1)
				{
					b = -1;
				}
			}
			else if (MyInfoManager.Instance.Slot >= 4)
			{
				if (val == 1)
				{
					b = -1;
				}
				if (val2 == 1)
				{
					b = 1;
				}
			}
			gameObject.BroadcastMessage("OnMatchEnd", b);
		}
	}

	private void HandleCS_INDIVIDUAL_MATCH_END_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		List<ResultUnit> list = new List<ResultUnit>();
		for (int i = 0; i < val; i++)
		{
			msg.Read(out bool val2);
			msg.Read(out int val3);
			msg.Read(out string val4);
			msg.Read(out int val5);
			msg.Read(out int val6);
			msg.Read(out int val7);
			msg.Read(out int val8);
			msg.Read(out int val9);
			msg.Read(out int val10);
			msg.Read(out int val11);
			msg.Read(out int val12);
			msg.Read(out int val13);
			msg.Read(out long val14);
			list.Add(new ResultUnit(val2, val3, val4, val5, val6, val7, val8, val9, val10, val11, val12, val13, val14));
		}
		list.Sort((ResultUnit prev, ResultUnit next) => prev.Compare(next));
		RoomManager.Instance.RU = list.ToArray();
		RoomManager.Instance.endCode = 0;
		RoomManager.Instance.redTotalKill = 0;
		RoomManager.Instance.redTotalDeath = 0;
		RoomManager.Instance.blueTotalKill = 0;
		RoomManager.Instance.blueTotalDeath = 0;
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnMatchEnd", (object)(sbyte)0);
		}
	}

	private void HandleCS_BLAST_MODE_END_ACK(MsgBody msg)
	{
		msg.Read(out sbyte val);
		msg.Read(out int val2);
		msg.Read(out int val3);
		msg.Read(out int val4);
		msg.Read(out int val5);
		msg.Read(out int val6);
		msg.Read(out int val7);
		msg.Read(out int val8);
		msg.Read(out int val9);
		msg.Read(out int val10);
		List<ResultUnit> list = new List<ResultUnit>();
		for (int i = 0; i < val10; i++)
		{
			msg.Read(out bool val11);
			msg.Read(out int val12);
			msg.Read(out string val13);
			msg.Read(out int val14);
			msg.Read(out int val15);
			msg.Read(out int val16);
			msg.Read(out int val17);
			msg.Read(out int val18);
			msg.Read(out int val19);
			msg.Read(out int val20);
			msg.Read(out int val21);
			msg.Read(out int val22);
			msg.Read(out long val23);
			list.Add(new ResultUnit(val11, val12, val13, val14, val15, val16, val17, val18, val19, val20, val21, val22, val23));
		}
		list.Sort((ResultUnit prev, ResultUnit next) => prev.Compare(next));
		RoomManager.Instance.RU = list.ToArray();
		RoomManager.Instance.endCode = val;
		RoomManager.Instance.redTotalKill = val6;
		RoomManager.Instance.redTotalDeath = val8;
		RoomManager.Instance.redScore = val2;
		RoomManager.Instance.redMission = val4;
		RoomManager.Instance.blueTotalKill = val7;
		RoomManager.Instance.blueTotalDeath = val9;
		RoomManager.Instance.blueScore = val3;
		RoomManager.Instance.blueMission = val5;
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			ClanMatchRounding component = gameObject.GetComponent<ClanMatchRounding>();
			if (null != component)
			{
				component.ShowRoundMessage = false;
			}
			ExplosionMatch component2 = gameObject.GetComponent<ExplosionMatch>();
			if (null != component2)
			{
				component2.ShowRoundMessage = false;
			}
			gameObject.BroadcastMessage("OnMatchEnd", val);
		}
	}

	private void HandleCS_CAPTURE_THE_FLAG_END_ACK(MsgBody msg)
	{
		msg.Read(out sbyte val);
		msg.Read(out int val2);
		msg.Read(out int val3);
		msg.Read(out int val4);
		msg.Read(out int val5);
		msg.Read(out int val6);
		msg.Read(out int val7);
		msg.Read(out int val8);
		List<ResultUnit> list = new List<ResultUnit>();
		for (int i = 0; i < val8; i++)
		{
			msg.Read(out bool val9);
			msg.Read(out int val10);
			msg.Read(out string val11);
			msg.Read(out int val12);
			msg.Read(out int val13);
			msg.Read(out int val14);
			msg.Read(out int val15);
			msg.Read(out int val16);
			msg.Read(out int val17);
			msg.Read(out int val18);
			msg.Read(out int val19);
			msg.Read(out int val20);
			msg.Read(out long val21);
			list.Add(new ResultUnit(val9, val10, val11, val12, val13, val14, val15, val16, val17, val18, val19, val20, val21));
		}
		list.Sort((ResultUnit prev, ResultUnit next) => prev.Compare(next));
		RoomManager.Instance.RU = list.ToArray();
		RoomManager.Instance.endCode = val;
		RoomManager.Instance.redTotalKill = val4;
		RoomManager.Instance.redTotalDeath = val6;
		RoomManager.Instance.redScore = val2;
		RoomManager.Instance.redMission = 0;
		RoomManager.Instance.blueTotalKill = val5;
		RoomManager.Instance.blueTotalDeath = val7;
		RoomManager.Instance.blueScore = val3;
		RoomManager.Instance.blueMission = 0;
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnMatchEnd", val);
		}
	}

	private void HandleCS_BND_MODE_END_ACK(MsgBody msg)
	{
		msg.Read(out sbyte val);
		msg.Read(out int val2);
		msg.Read(out int val3);
		msg.Read(out int val4);
		msg.Read(out int val5);
		msg.Read(out int val6);
		List<ResultUnit> list = new List<ResultUnit>();
		for (int i = 0; i < val6; i++)
		{
			msg.Read(out bool val7);
			msg.Read(out int val8);
			msg.Read(out string val9);
			msg.Read(out int val10);
			msg.Read(out int val11);
			msg.Read(out int val12);
			msg.Read(out int val13);
			msg.Read(out int val14);
			msg.Read(out int val15);
			msg.Read(out int val16);
			msg.Read(out int val17);
			msg.Read(out int val18);
			msg.Read(out long val19);
			list.Add(new ResultUnit(val7, val8, val9, val10, val11, val12, val13, val14, val15, val16, val17, val18, val19));
		}
		list.Sort((ResultUnit prev, ResultUnit next) => prev.Compare(next));
		RoomManager.Instance.RU = list.ToArray();
		RoomManager.Instance.endCode = val;
		RoomManager.Instance.redTotalKill = val2;
		RoomManager.Instance.redTotalDeath = val3;
		RoomManager.Instance.redScore = 0;
		RoomManager.Instance.blueTotalKill = val4;
		RoomManager.Instance.blueTotalDeath = val5;
		RoomManager.Instance.blueScore = 0;
		Squad curSquad = SquadManager.Instance.CurSquad;
		if (curSquad != null && MyInfoManager.Instance.Seq == curSquad.Leader)
		{
			int num = curSquad.WinCount;
			int num2 = curSquad.LoseCount;
			int num3 = curSquad.DrawCount;
			if (val < 0)
			{
				num2++;
			}
			else if (val > 0)
			{
				num++;
			}
			else
			{
				num3++;
			}
			CSNetManager.Instance.Sock.SendCS_UPDATE_SQUAD_RECORD_REQ(num, num3, num2);
		}
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnMatchEnd", val);
		}
	}

	private void HandleCS_TEAM_MATCH_END_ACK(MsgBody msg)
	{
		msg.Read(out sbyte val);
		msg.Read(out int val2);
		msg.Read(out int val3);
		msg.Read(out int val4);
		msg.Read(out int val5);
		msg.Read(out int val6);
		List<ResultUnit> list = new List<ResultUnit>();
		for (int i = 0; i < val6; i++)
		{
			msg.Read(out bool val7);
			msg.Read(out int val8);
			msg.Read(out string val9);
			msg.Read(out int val10);
			msg.Read(out int val11);
			msg.Read(out int val12);
			msg.Read(out int val13);
			msg.Read(out int val14);
			msg.Read(out int val15);
			msg.Read(out int val16);
			msg.Read(out int val17);
			msg.Read(out int val18);
			msg.Read(out long val19);
			list.Add(new ResultUnit(val7, val8, val9, val10, val11, val12, val13, val14, val15, val16, val17, val18, val19));
		}
		list.Sort((ResultUnit prev, ResultUnit next) => prev.Compare(next));
		RoomManager.Instance.RU = list.ToArray();
		RoomManager.Instance.endCode = val;
		RoomManager.Instance.redTotalKill = val2;
		RoomManager.Instance.redTotalDeath = val3;
		RoomManager.Instance.redScore = 0;
		RoomManager.Instance.blueTotalKill = val4;
		RoomManager.Instance.blueTotalDeath = val5;
		RoomManager.Instance.blueScore = 0;
		Squad curSquad = SquadManager.Instance.CurSquad;
		if (curSquad != null && MyInfoManager.Instance.Seq == curSquad.Leader)
		{
			int num = curSquad.WinCount;
			int num2 = curSquad.LoseCount;
			int num3 = curSquad.DrawCount;
			if (val < 0)
			{
				num2++;
			}
			else if (val > 0)
			{
				num++;
			}
			else
			{
				num3++;
			}
			CSNetManager.Instance.Sock.SendCS_UPDATE_SQUAD_RECORD_REQ(num, num3, num2);
		}
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnMatchEnd", val);
		}
	}

	private void HandleCS_KILL_COUNT_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		if (val == MyInfoManager.Instance.Seq)
		{
			MyInfoManager.Instance.Kill = val2;
		}
		else
		{
			BrickManDesc desc = BrickManManager.Instance.GetDesc(val);
			if (desc != null)
			{
				desc.Kill = val2;
			}
		}
	}

	private void HandleCS_ME_REG_BRICK_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		for (int i = 0; i < val; i++)
		{
			msg.Read(out int val2);
			UserMapInfoManager.Instance.CacheRegMapBrick(val2);
		}
	}

	private void HandleCS_ME_REG_BRICK_END_ACK(MsgBody msg)
	{
		msg.Read(out bool val);
		UserMapInfoManager.Instance.CacheRegMapBrickDone(val);
	}

	private void HandleCS_DELETED_MAP_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		RegMapManager.Instance.LogDeletedMap(val);
	}

	private void HandleCS_DELETED_MAP_LIST_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		for (int i = 0; i < val; i++)
		{
			msg.Read(out int val2);
			RegMapManager.Instance.LogDeletedMap(val2);
		}
	}

	private void HandleCS_BND_SCORE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnTeamScore", new TeamScore(val, val2));
		}
	}

	private void HandleCS_TEAM_SCORE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnTeamScore", new TeamScore(val, val2));
		}
	}

	private void HandleCS_INDIVIDUAL_SCORE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnIndividualScore", val);
		}
	}

	private void HandleCS_MATCH_COUNTDOWN_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnMatchCountDown", val);
		}
	}

	private void HandleCS_KICK_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		BrickManDesc desc = BrickManManager.Instance.GetDesc(val);
		if (desc != null)
		{
			MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("HAS_BEEN_KICKED"), desc.Nickname));
		}
		else if (val == MyInfoManager.Instance.Seq)
		{
			_waitingAck = false;
			P2PManager.Instance.Shutdown();
			DialogManager.Instance.CloseAll();
			ContextMenuManager.Instance.CloseAll();
			MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("HAS_BEEN_KICKED"), MyInfoManager.Instance.Nickname));
			SendCS_MAKE_ROOM_NULL_REQ();
			Application.LoadLevel("Lobby");
		}
	}

	private void HandleCS_SEED_ACK(MsgBody msg)
	{
		msg.Read(out byte val);

        recvKey = val;
		srandMine(val);
		if (val == 255)
		{
			sendKeys = null;
		}
		else
		{
			int num = randMine() % 10 + 10;
			sendKeys = new byte[num];
			for (int i = 0; i < num; i++)
			{
				sendKeys[i] = (byte)randMine();
			}
		}

        _heartbeat = true;
		GlobalVars.Instance.initcm();
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnSeed");
		}
	}

	private void HandleCS_NEW_MAP_LIST_ACK(MsgBody msg)
	{
	}

	private void HandleCS_RANK_MAP_LIST_ACK(MsgBody msg)
	{
	}

	private void HandleCS_MY_MAP_LIST_ACK(MsgBody msg)
	{
	}

	private void HandleCS_DOWNLOAD_THUMBNAIL_END_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		ThumbnailRequest thumbnailRequest = ThumbnailDownloader.Instance.Dequeue(val >= 0);
		if (thumbnailRequest != null)
		{
			if (val < 0)
			{
				if (thumbnailRequest.IsUserMap)
				{
					Debug.LogError("Fail to download thumbnail for UserMap" + thumbnailRequest.Id);
				}
				else
				{
					Debug.LogError("Fail to download thumbnail for RegMap" + thumbnailRequest.Id);
				}
			}
			else if (val == thumbnailRequest.Id)
			{
				Texture2D texture2D = new Texture2D(128, 128, TextureFormat.RGB24, mipmap: false);
				texture2D.LoadImage(thumbnailRequest.ThumbnailBuffer);
				texture2D.Apply();
				if (!thumbnailRequest.IsUserMap)
				{
					RegMapManager.Instance.SetThumbnail(val, texture2D);
				}
				else
				{
					UserMapInfoManager.Instance.SetThumbnail((byte)val, texture2D);
				}
			}
		}
	}

	private void HandleCS_DOWNLOAD_THUMBNAIL_ACK(MsgBody msg)
	{
		msg.Read(out int _);
		msg.Read(out int val2);
		if (val2 > 0)
		{
			byte[] array = new byte[val2];
			for (int i = 0; i < val2; i++)
			{
				msg.Read(out array[i]);
			}
			ThumbnailDownloader.Instance.Stack(array);
		}
	}

	private void HandleCS_CHANGE_USERMAP_ALIAS_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out sbyte val2);
		msg.Read(out string val3);
		if (val == 1)
		{
			UserMapInfo userMapInfo = UserMapInfoManager.Instance.Get((byte)val2);
			if (userMapInfo != null)
			{
				userMapInfo.Alias = val3;
				MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("RENAME_SUCCESS"), val3));
			}
		}
		else
		{
			MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("RENAME_FAILED"), val3));
		}
	}

	private void HandleCS_ADD_CLANEE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out string val2);
		msg.Read(out int val3);
		msg.Read(out int val4);
		msg.Read(out int val5);
		MyInfoManager.Instance.AddClanee(val, val2, XpManager.Instance.GetLevel(val3), val5, val4);
	}

	private void HandleCS_ADD_FRIEND_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out string val2);
		msg.Read(out int val3);
		msg.Read(out int val4);
		msg.Read(out int val5);
		MyInfoManager.Instance.AddFriend(val, val2, XpManager.Instance.GetLevel(val3), val5, val4);
	}

	private void HandleCS_ADD_BAN_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out string val2);
		MyInfoManager.Instance.AddBan(val, val2);
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, string.Empty, string.Format(StringMgr.Instance.Get("BAN_PLAYER_MESSAGE01"), val2)));
		}
	}

	private void HandleCS_DEL_FRIEND_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		MyInfoManager.Instance.DelFriend(val);
	}

	private void HandleCS_DEL_BAN_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		NameCard ban = MyInfoManager.Instance.GetBan(val);
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject && ban != null)
		{
			gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, string.Empty, string.Format(StringMgr.Instance.Get("BAN_PLAYER_MESSAGE02"), ban.Nickname)));
		}
		MyInfoManager.Instance.DelBan(val);
	}

	private void HandleCS_ADD_FRIEND_FAIL_ACK(MsgBody msg)
	{
		msg.Read(out int _);
		MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("FAIL_TO_ADD_FRIEND"));
	}

	private void HandleCS_DEL_FRIEND_FAIL_ACK(MsgBody msg)
	{
		msg.Read(out int _);
		MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("FAIL_TO_DEL_FRIEND"));
	}

	private void HandleCS_ADD_BAN_FAIL_ACK(MsgBody msg)
	{
		msg.Read(out int _);
		MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("FAIL_TO_ADD_BAN"));
	}

	private void HandleCS_DEL_BAN_FAIL_ACK(MsgBody msg)
	{
		msg.Read(out int _);
		MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("FAIL_TO_DEL_BAN"));
	}

	private void HandleCS_ADD_FRIEND_BY_NICKNAME_FAIL_ACK(MsgBody msg)
	{
		msg.Read(out int _);
		msg.Read(out string val2);
		MessageBoxMgr.Instance.AddMessage("[" + val2 + "] " + StringMgr.Instance.Get("FAIL_TO_ADD_FRIEND"));
	}

	private void HandleCS_ADD_BAN_BY_NICKNAME_FAIL_ACK(MsgBody msg)
	{
		msg.Read(out int _);
		msg.Read(out string val2);
		MessageBoxMgr.Instance.AddMessage("[" + val2 + "] " + StringMgr.Instance.Get("FAIL_TO_ADD_BAN"));
	}

	private void HandleCS_RCV_PRESENT_ACK(MsgBody msg)
	{
		msg.Read(out long val);
		msg.Read(out long val2);
		msg.Read(out string val3);
		msg.Read(out int val4);
		msg.Read(out int val5);
		if (val >= 0)
		{
			int option = MemoManager.Instance.GetOption(val2);
			MyInfoManager.Instance.ReceivePresentItem(val, val3, val4, val5);
			MemoManager.Instance.ClearPresent(val2);
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				TItem tItem = TItemManager.Instance.Get<TItem>(val3);
				if (tItem != null)
				{
					string text = string.Format(StringMgr.Instance.Get("MEMO_BOX_ITEM"), tItem.Name, tItem.GetOptionStringByOption(option));
					gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, string.Empty, text));
				}
			}
		}
		else
		{
			long num = val;
			if (num >= -4 && num <= -1)
			{
				switch (num - -4)
				{
				case 3L:
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NO_SUCH_ITEM2BUY"));
					break;
				case 2L:
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("HAVEINFINITEITEM"));
					break;
				case 1L:
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NOT_ENOUGH_MONEY"));
					break;
				case 0L:
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("FAIL2BUY_UNKNOWN_ERROR"));
					break;
				}
			}
		}
		_waitingAck = false;
		if (DialogManager.Instance.IsPopup(DialogManager.DIALOG_INDEX.MEMO))
		{
			((MemoDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.MEMO))?.NextMemoReceiceItem();
		}
	}

	private void HandleCS_ERASE_DELETED_ITEM_ACK(MsgBody msg)
	{
		msg.Read(out long val);
		if (val >= 0)
		{
			MyInfoManager.Instance.Erase(val);
		}
	}

	private void HandleCS_DISCOMPOSE_ITEM_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out long val2);
		if (val == 0)
		{
			GlobalVars.Instance.bEraseItemOk = true;
			MyInfoManager.Instance.Erase(val2);
		}
	}

	private void HandleCS_RENDEZVOUS_INFO_ACK(MsgBody msg)
	{
		msg.Read(out int _);
		msg.Read(out string val2);
		msg.Read(out int val3);
		P2PManager.Instance.Bootup(val2, val3);
	}

	private void HandleCS_USERMAP_END_ACK(MsgBody msg)
	{
		UserMapInfoManager.Instance.ValidateEmpty();
	}

	private void HandleCS_STAR_LEVEL_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		for (int i = 0; i < val; i++)
		{
			msg.Read(out int val2);
			msg.Read(out int val3);
			XpManager.Instance.SetStarLevel(val2, val3);
		}
	}

	private void HandleCS_SHOP_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		for (int i = 0; i < val; i++)
		{
			msg.Read(out string val2);
			msg.Read(out sbyte val3);
			msg.Read(out sbyte val4);
			msg.Read(out sbyte val5);
			msg.Read(out byte val6);
			msg.Read(out sbyte val7);
			msg.Read(out sbyte val8);
			msg.Read(out sbyte val9);
			msg.Read(out sbyte val10);
			msg.Read(out int val11);
			msg.Read(out int val12);
			msg.Read(out int val13);
			msg.Read(out int val14);
			msg.Read(out int val15);
			msg.Read(out int val16);
			msg.Read(out int val17);
			msg.Read(out int val18);
			msg.Read(out int val19);
			msg.Read(out int val20);
			msg.Read(out int val21);
			msg.Read(out int val22);
			msg.Read(out sbyte val23);
			msg.Read(out sbyte val24);
			msg.Read(out sbyte val25);
			ShopManager.Instance.Cache(val2, val3, val4, val5, val6, val7, val8, val9, val10, val11, val12, val13, val14, val15, val16, val17, val18, val19, val20, val21, val22, val23, val24, val25);
		}
		ShopManager.Instance.CacheDone();
	}

	private void HandleCS_SHOP_END_ACK(MsgBody msg)
	{
		ShopManager.Instance.CacheDone();
	}

	private void HandleCS_WEAPON_MODIFIER_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		for (int i = 0; i < val; i++)
		{
			msg.Read(out int val2);
			msg.Read(out float val3);
			msg.Read(out float val4);
			msg.Read(out float val5);
			msg.Read(out float val6);
			msg.Read(out float val7);
			msg.Read(out float val8);
			msg.Read(out float val9);
			msg.Read(out float val10);
			msg.Read(out float val11);
			msg.Read(out float val12);
			msg.Read(out float val13);
			msg.Read(out float val14);
			msg.Read(out float val15);
			msg.Read(out float val16);
			msg.Read(out float val17);
			msg.Read(out float val18);
			msg.Read(out float val19);
			msg.Read(out float val20);
			msg.Read(out float val21);
			msg.Read(out float val22);
			msg.Read(out float val23);
			msg.Read(out float val24);
			msg.Read(out float val25);
			msg.Read(out float val26);
			msg.Read(out float val27);
			msg.Read(out float val28);
			msg.Read(out float val29);
			msg.Read(out float val30);
			msg.Read(out float val31);
			msg.Read(out float val32);
			msg.Read(out float val33);
			msg.Read(out float val34);
			msg.Read(out int val35);
			msg.Read(out int val36);
			msg.Read(out float val37);
			msg.Read(out float val38);
			msg.Read(out float val39);
			msg.Read(out float val40);
			msg.Read(out float val41);
			WeaponModifier.Instance.UpdateWpnMod(val2, val3, val4, val5, val6, val7, val8, val9, val10, val11, val12, val13, val14, val15, val16, val17, val18, val19, val20, val21, val22, val23, val24, val25, val26, val27, val28, val29, val30, val31, val32, val33, val34, val35, val36, val37, val38, val39, val40, val41);
		}
	}

	private void HandleCS_ME_PREMIUM_BRICKS_ACK(MsgBody msg)
	{
		msg.Read(out long val);
		RoomManager.Instance.PremiumBricks = val;
	}

	private void HandleCS_WEAPON_LEVEL_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out int val3);
		msg.Read(out int val4);
		msg.Read(out int val5);
		msg.Read(out int val6);
		msg.Read(out int val7);
		int weaponLevel = XpManager.Instance.GetWeaponLevel(TWeapon.CATEGORY.HEAVY, MyInfoManager.Instance.Heavy);
		int weaponLevel2 = XpManager.Instance.GetWeaponLevel(TWeapon.CATEGORY.ASSAULT, MyInfoManager.Instance.Assault);
		int weaponLevel3 = XpManager.Instance.GetWeaponLevel(TWeapon.CATEGORY.SNIPER, MyInfoManager.Instance.Sniper);
		int weaponLevel4 = XpManager.Instance.GetWeaponLevel(TWeapon.CATEGORY.SUB_MACHINE, MyInfoManager.Instance.SubMachine);
		int weaponLevel5 = XpManager.Instance.GetWeaponLevel(TWeapon.CATEGORY.HAND_GUN, MyInfoManager.Instance.HandGun);
		int weaponLevel6 = XpManager.Instance.GetWeaponLevel(TWeapon.CATEGORY.MELEE, MyInfoManager.Instance.Melee);
		int weaponLevel7 = XpManager.Instance.GetWeaponLevel(TWeapon.CATEGORY.SPECIAL, MyInfoManager.Instance.Special);
		MyInfoManager.Instance.Heavy = val;
		MyInfoManager.Instance.Assault = val2;
		MyInfoManager.Instance.Sniper = val3;
		MyInfoManager.Instance.SubMachine = val4;
		MyInfoManager.Instance.HandGun = val5;
		MyInfoManager.Instance.Melee = val6;
		MyInfoManager.Instance.Special = val7;
		int weaponLevel8 = XpManager.Instance.GetWeaponLevel(TWeapon.CATEGORY.HEAVY, MyInfoManager.Instance.Heavy);
		int weaponLevel9 = XpManager.Instance.GetWeaponLevel(TWeapon.CATEGORY.ASSAULT, MyInfoManager.Instance.Assault);
		int weaponLevel10 = XpManager.Instance.GetWeaponLevel(TWeapon.CATEGORY.SNIPER, MyInfoManager.Instance.Sniper);
		int weaponLevel11 = XpManager.Instance.GetWeaponLevel(TWeapon.CATEGORY.SUB_MACHINE, MyInfoManager.Instance.SubMachine);
		int weaponLevel12 = XpManager.Instance.GetWeaponLevel(TWeapon.CATEGORY.HAND_GUN, MyInfoManager.Instance.HandGun);
		int weaponLevel13 = XpManager.Instance.GetWeaponLevel(TWeapon.CATEGORY.MELEE, MyInfoManager.Instance.Melee);
		int weaponLevel14 = XpManager.Instance.GetWeaponLevel(TWeapon.CATEGORY.SPECIAL, MyInfoManager.Instance.Special);
		if (weaponLevel8 > weaponLevel)
		{
			MyInfoManager.Instance.EnResultEvent(MyInfoManager.RESULT_EVENT.HEAVY_LEVEL_UP);
		}
		if (weaponLevel9 > weaponLevel2)
		{
			MyInfoManager.Instance.EnResultEvent(MyInfoManager.RESULT_EVENT.ASSAULT_LEVEL_UP);
		}
		if (weaponLevel10 > weaponLevel3)
		{
			MyInfoManager.Instance.EnResultEvent(MyInfoManager.RESULT_EVENT.SNIPER_LEVEL_UP);
		}
		if (weaponLevel11 > weaponLevel4)
		{
			MyInfoManager.Instance.EnResultEvent(MyInfoManager.RESULT_EVENT.SUBMACHINE_LEVEL_UP);
		}
		if (weaponLevel12 > weaponLevel5)
		{
			MyInfoManager.Instance.EnResultEvent(MyInfoManager.RESULT_EVENT.HANDGUN_LEVEL_UP);
		}
		if (weaponLevel13 > weaponLevel6)
		{
			MyInfoManager.Instance.EnResultEvent(MyInfoManager.RESULT_EVENT.MELEE_LEVEL_UP);
		}
		if (weaponLevel14 > weaponLevel7)
		{
			MyInfoManager.Instance.EnResultEvent(MyInfoManager.RESULT_EVENT.SPECIAL_LEVEL_UP);
		}
	}

	private void HandleCS_REBUY_ITEM_ACK(MsgBody msg)
	{
		msg.Read(out long val);
		msg.Read(out string val2);
		msg.Read(out int val3);
		msg.Read(out int val4);
		if (val >= 0)
		{
			MyInfoManager.Instance.RebuyItem(val, val2, val3, val4);
			TItem tItem = TItemManager.Instance.Get<TItem>(val2);
			if (tItem != null)
			{
				string msg2 = string.Format(StringMgr.Instance.Get("BUY_SUCCESS"), tItem.Name);
				SystemMsgManager.Instance.ShowMessage(msg2);
				if (val2.Equals("s22"))
				{
					PremiumItemManager.Instance.ResetPremiumItems();
				}
			}
		}
		else
		{
			long num = val;
			if (num >= -4 && num <= -1)
			{
				switch (num - -4)
				{
				case 3L:
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NO_SUCH_ITEM2BUY"));
					return;
				case 2L:
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("HAVEINFINITEITEM"));
					return;
				case 1L:
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NOT_ENOUGH_MONEY"));
					return;
				case 0L:
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("FAIL2BUY_UNKNOWN_ERROR"));
					return;
				}
			}
			switch (num)
			{
			case -114L:
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("ITEM_PURCHASE_LIMIT_MSG01"));
				break;
			case -115L:
				MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("ITEM_PURCHASE_LIMIT_MSG02"), TokenManager.Instance.GetTokenString()));
				break;
			}
		}
	}

	private void HandleCS_BUY_ITEM_ACK(MsgBody msg)
	{
        // val = sequence (unique key)
        // val2 = code
        // val3 = remain (time in days)
        // val4 = premium 0 || 1
        // val5 = durability
		msg.Read(out long val);
		msg.Read(out string val2);
		msg.Read(out int val3);
		msg.Read(out sbyte val4);
		msg.Read(out int val5);
        if (val >= 0)
		{
			MyInfoManager.Instance.BuyItem(val, val2, val3, val4, val5);
			TItem tItem = TItemManager.Instance.Get<TItem>(val2);
			if (tItem != null)
			{
				Good good = ShopManager.Instance.Get(val2);
				if (good != null && good.isOfferOnce)
				{
					ShopManager.Instance.LogBought(val2);
				}
				if (good != null)
				{
					if (!good.Check)
					{
						string msg2 = string.Format(StringMgr.Instance.Get("BUY_SUCCESS"), tItem.Name);
						SystemMsgManager.Instance.ShowMessage(msg2);
					}
					else
					{
						good.Check = false;
					}
				}
				if (val2.Equals("s22"))
				{
					PremiumItemManager.Instance.ResetPremiumItems();
				}
			}
		}
		else
		{
			Good good2 = ShopManager.Instance.Get(val2);
			if (good2 != null)
			{
				good2.Check = false;
			}
			if (!DialogManager.Instance.IsPopup(DialogManager.DIALOG_INDEX.MBUY_TERM))
			{
				long num = val;
				if (num >= -115 && num <= -111)
				{
					switch (num - -115)
					{
					case 4L:
						MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("BOUGHT_ALREADY_OFFER_ONCE"));
						return;
					case 2L:
						MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("PROMO_NOT_AVAILABLE"));
						return;
					case 1L:
						MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("ITEM_PURCHASE_LIMIT_MSG01"));
						return;
					case 0L:
						MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("ITEM_PURCHASE_LIMIT_MSG02"), TokenManager.Instance.GetTokenString()));
						return;
					}
				}
				if (num >= -4 && num <= -1)
				{
					switch (num - -4)
					{
					case 3L:
						MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NO_SUCH_ITEM2BUY"));
						break;
					case 2L:
						MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("HAVEINFINITEITEM"));
						break;
					case 1L:
						MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NOT_ENOUGH_MONEY"));
						break;
					case 0L:
						MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("FAIL2BUY_UNKNOWN_ERROR"));
						break;
					}
				}
			}
			else
			{
				long num = val;
				if (num >= -115 && num <= -111)
				{
					switch (num - -115)
					{
					case 4L:
						good2.BuyErr = StringMgr.Instance.Get("BOUGHT_ALREADY_OFFER_ONCE");
						return;
					case 2L:
						good2.BuyErr = StringMgr.Instance.Get("PROMO_NOT_AVAILABLE");
						return;
					case 1L:
						good2.BuyErr = StringMgr.Instance.Get("ITEM_PURCHASE_LIMIT_MSG01");
						return;
					case 0L:
						good2.BuyErr = string.Format(StringMgr.Instance.Get("ITEM_PURCHASE_LIMIT_MSG02"), TokenManager.Instance.GetTokenString());
						return;
					}
				}
				if (num >= -4 && num <= -1)
				{
					switch (num - -4)
					{
					case 3L:
						good2.BuyErr = StringMgr.Instance.Get("NO_SUCH_ITEM2BUY");
						break;
					case 2L:
						good2.BuyErr = StringMgr.Instance.Get("HAVEINFINITEITEM");
						break;
					case 1L:
						good2.BuyErr = StringMgr.Instance.Get("NOT_ENOUGH_MONEY");
						break;
					case 0L:
						good2.BuyErr = StringMgr.Instance.Get("FAIL2BUY_UNKNOWN_ERROR");
						break;
					}
				}
			}
		}
	}

	private void HandleCS_TERM_ITEM_EXPIRED_ACK(MsgBody msg)
	{
		msg.Read(out long val);
		MyInfoManager.Instance.TermItemExpired(val);
	}

	private void HandleCS_WEAPON_DURABILITY_ACK(MsgBody msg)
	{
		msg.Read(out long val);
		msg.Read(out int val2);
		Item itemBySequence = MyInfoManager.Instance.GetItemBySequence(val);
		if (itemBySequence != null)
		{
			int durability = itemBySequence.Durability;
			itemBySequence.Durability = val2;
			if (durability != val2)
			{
				MyInfoManager.Instance.EnDurabilityEvent(itemBySequence.Code, val2, durability - val2);
			}
		}
	}

	private void HandleCS_REPAIR_WEAPON_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out long val2);
		string empty = string.Empty;
		Item itemBySequence = MyInfoManager.Instance.GetItemBySequence(val2);
		if (itemBySequence != null)
		{
			empty = "[" + itemBySequence.Template.Name + "]";
			switch (val)
			{
			case 0:
				MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("REPAIR_OK"), empty));
				break;
			case -1:
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("ITEM_NOT_FOUND"));
				break;
			case -2:
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NO_REPAIR"));
				break;
			case -3:
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NOT_ENOUGH_MONEY_REPAIR"));
				break;
			}
		}
	}

	private void HandleCS_UPGRADE_ITEM_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out long _);
		msg.Read(out long _);
		msg.Read(out int val4);
		msg.Read(out int val5);
		switch (val)
		{
		case 0:
			GlobalVars.Instance.successUpgradePropID = val4;
			break;
		default:
			switch (val)
			{
			case -1:
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("ITEMUPGRADE_FAIL"));
				break;
			case -2:
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("ITEMUPGRADE_FAIL"));
				break;
			case -3:
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("ITEMUPGRADE_FAIL"));
				break;
			case -4:
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("ITEMUPGRADE_FAIL"));
				break;
			case -5:
			{
				string text = StringMgr.Instance.Get("UPGRADE_TIER_LEVEL");
				string rank = XpManager.Instance.GetRank(val5, -1);
				if (rank.Length > 0)
				{
					text = text + "\n" + string.Format(StringMgr.Instance.Get("UPGRADE_GEM_IS_POSSIBLE_FROM"), rank);
				}
				MessageBoxMgr.Instance.AddMessage(text);
				break;
			}
			case -6:
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("UPGRADE_TIER_NOT_FULL"));
				break;
			case -7:
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("ITEMUPGRADE_FAIL"));
				break;
			case -8:
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("ITEMUPGRADE_FAIL"));
				break;
			}
			break;
		case 1:
			break;
		}
		ItemUpgradeDlg itemUpgradeDlg = (ItemUpgradeDlg)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.ITEM_UPGRADE);
		if (itemUpgradeDlg != null)
		{
			switch (val)
			{
			case -6:
			case -5:
				break;
			case 0:
				itemUpgradeDlg.UpgradeResultSetting(result: true);
				break;
			default:
				itemUpgradeDlg.UpgradeResultSetting(result: false);
				break;
			}
		}
	}

	private void HandleCS_ITEM_PIMP_ACK(MsgBody msg)
	{
		msg.Read(out long val);
		msg.Read(out int val2);
		msg.Read(out int val3);
		Item itemBySequence = MyInfoManager.Instance.GetItemBySequence(val);
		if (itemBySequence != null)
		{
			itemBySequence.upgradeProps[val2].use = true;
			itemBySequence.upgradeProps[val2].grade = val3;
		}
	}

	private void HandleCS_BUNDLE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		List<string> list = new List<string>();
		for (int i = 0; i < val; i++)
		{
			msg.Read(out string val2);
			msg.Read(out string val3);
			msg.Read(out int val4);
			if (!list.Contains(val2))
			{
				BundleManager.Instance.Remove(val2);
				list.Add(val2);
			}
			BundleManager.Instance.Pack(val2, val3, val4);
		}
	}

	private void HandleCS_PROMO_LIST_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		for (int i = 0; i < val; i++)
		{
			msg.Read(out string val2);
			ShopManager.Instance.LogPromo(val2);
		}
	}

	private void HandleCS_BOUGHTONCE_LIST_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		for (int i = 0; i < val; i++)
		{
			msg.Read(out string val2);
			ShopManager.Instance.LogBought(val2);
		}
	}

	private void HandleCS_UNPACK_BUNDLE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		if (val != 0)
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("UNPACK_FAIL"));
		}
	}

	private void HandleCS_PIMP_MODIFIER_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		for (int i = 0; i < val; i++)
		{
			msg.Read(out int val2);
			msg.Read(out float val3);
			int cat = val2 >> 16;
			int prop = (val2 >> 8) & 0xFF;
			int num = val2 & 0xFF;
			PimpManager.Instance.updateValue(cat, prop, num - 1, val3);
		}
	}

	private void HandleCS_MERGE_ITEM_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out long val2);
		msg.Read(out long val3);
		msg.Read(out int val4);
		GlobalVars.Instance.bReceivedAck = true;
		if (val == 0)
		{
			MyInfoManager.Instance.Erase(val2);
			Item itemBySequence = MyInfoManager.Instance.GetItemBySequence(val3);
			itemBySequence.Remain = val4;
			if (itemBySequence.Usage == Item.USAGE.DELETED)
			{
				itemBySequence.Usage = Item.USAGE.UNEQUIP;
			}
			SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("COMPOSE_SUCCESS"));
		}
		else
		{
			SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("COMPOSE_FAIL"));
		}
		ItemCombineDialog itemCombineDialog = null;
		if (DialogManager.Instance.IsPopup(DialogManager.DIALOG_INDEX.ITEM_COMBINE))
		{
			itemCombineDialog = (ItemCombineDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.ITEM_COMBINE);
			if (val == 0)
			{
				itemCombineDialog.CombineEnd();
			}
			else
			{
				itemCombineDialog.CombineFail();
			}
		}
	}

	private void HandleCS_ITEM_ACK(MsgBody msg)
	{
		msg.Read(out long val);
		msg.Read(out string val2);
		msg.Read(out sbyte val3);
		msg.Read(out int val4);
		msg.Read(out sbyte val5);
		msg.Read(out int val6);
		MyInfoManager.Instance.SetItem(val, val2, (Item.USAGE)val3, val4, val5, val6);
		if (val2.Equals("s22"))
		{
			PremiumItemManager.Instance.ResetPremiumItems();
		}
	}

	private void HandleCS_ITEM_LIST_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		for (int i = 0; i < val; i++)
		{
			msg.Read(out long val2);
			msg.Read(out string val3);
			msg.Read(out sbyte val4);
			msg.Read(out int val5);
			msg.Read(out sbyte val6);
			msg.Read(out int val7);
			MyInfoManager.Instance.SetItem(val2, val3, (Item.USAGE)val4, val5, val6, val7);
		}
	}

	private void HandleCS_MEMO_ACK(MsgBody msg)
	{
		msg.Read(out long val);
		msg.Read(out string val2);
		msg.Read(out string val3);
		msg.Read(out string val4);
		msg.Read(out string val5);
		msg.Read(out int val6);
		msg.Read(out int val7);
		msg.Read(out sbyte val8);
		msg.Read(out sbyte val9);
		msg.Read(out sbyte val10);
		msg.Read(out sbyte val11);
		MemoManager.Instance.Add(val, new Memo(val, val2, val3, val4, val5, val6, val7, val8, val9, val10, val11));
	}

	private void HandleCS_SEND_MEMO_ACK(MsgBody msg)
	{
		msg.Read(out long val);
		msg.Read(out string val2);
		if (val > 0)
		{
			MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("SEND_MEMO_SUCCESS"), val2));
		}
		else
		{
			long num = val;
			if (num >= -3 && num <= -1)
			{
				switch (num - -3)
				{
				case 2L:
					MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("USER_NOT_FOUND"), val2));
					break;
				case 1L:
					MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("SEND_MEMO_FAIL_FULL"), val2));
					break;
				case 0L:
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("JUST_SEND_MEMO_FAIL"));
					break;
				}
			}
		}
	}

	private void HandleCS_PRESENT_ITEM_ACK(MsgBody msg)
	{
		msg.Read(out long val);
		msg.Read(out string val2);
		msg.Read(out string val3);
		if (val >= 0)
		{
			SystemMsgManager.Instance.ShowMessage(string.Format(StringMgr.Instance.Get("PRESENT_ITEM_SUCCESS"), val3));
			Good good = ShopManager.Instance.Get(val2);
			if (good != null && good.isOfferOnce)
			{
				ShopManager.Instance.LogBought(val2);
			}
		}
		else
		{
			long num = val;
			if (num >= -117 && num <= -111)
			{
				switch (num - -117)
				{
				case 6L:
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("BOUGHT_ALREADY_OFFER_ONCE"));
					return;
				case 5L:
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NO_GIFTABLE"));
					return;
				case 4L:
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("PROMO_NOT_AVAILABLE"));
					return;
				case 3L:
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("ITEM_PURCHASE_LIMIT_MSG01"));
					return;
				case 2L:
					MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("ITEM_PURCHASE_LIMIT_MSG02"), TokenManager.Instance.GetTokenString()));
					return;
				case 1L:
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("ITEM_PURCHASE_LIMIT_MSG03"));
					return;
				case 0L:
					MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("ITEM_PURCHASE_LIMIT_MSG04"), TokenManager.Instance.GetTokenString()));
					return;
				}
			}
			if (num >= -5 && num <= -1)
			{
				switch (num - -5)
				{
				case 4L:
					MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("USER_NOT_FOUND"), val3));
					break;
				case 3L:
					MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("PRESENT_ITEM_FAIL_FULL"), val3));
					break;
				case 2L:
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("JUST_PRESENT_ITEM_FAIL"));
					break;
				case 1L:
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NO_SUCH_ITEM2BUY"));
					break;
				case 0L:
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NOT_ENOUGH_MONEY"));
					break;
				}
			}
		}
	}

	private void HandleCS_DEL_MEMO_ACK(MsgBody msg)
	{
		msg.Read(out long val);
		MemoManager.Instance.Del(val);
		_waitingAck = false;
		if (DialogManager.Instance.IsPopup(DialogManager.DIALOG_INDEX.MEMO))
		{
			((MemoDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.MEMO))?.NextMemoDelete();
		}
	}

	private void HandleCS_P2P_COMPLETE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		if (val == MyInfoManager.Instance.Seq)
		{
			MyInfoManager.Instance.Status = 4;
		}
		else
		{
			BrickManDesc desc = BrickManManager.Instance.GetDesc(val);
			if (desc != null)
			{
				desc.Status = 4;
			}
		}
		GameObject gameObject = GameObject.Find("Main");
		if (gameObject != null)
		{
			MapEditor component = gameObject.GetComponent<MapEditor>();
			if (component != null)
			{
				component.RemoveLoadPlayer(val);
			}
		}
	}

	private void HandleCS_RESULT_DONE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		if (val == MyInfoManager.Instance.Seq)
		{
			MyInfoManager.Instance.Status = 0;
			P2PManager.Instance.EndSession();
		}
		else
		{
			BrickManDesc desc = BrickManManager.Instance.GetDesc(val);
			if (desc != null)
			{
				desc.Status = 0;
			}
		}
		if (ChannelManager.Instance.CurChannel.Mode == 4)
		{
			GlobalVars.Instance.clanTeamMatchSuccess = 1;
		}
	}

	private void HandleCS_CHANNEL_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		for (int i = 0; i < val; i++)
		{
			try
			{
				msg.Read(out int val2);
				msg.Read(out int val3);
				msg.Read(out string val4);
				msg.Read(out string val5);
				msg.Read(out int val6);
				msg.Read(out int val7);
				msg.Read(out int val8);
				msg.Read(out int val9);
				msg.Read(out byte val10);
				msg.Read(out byte val11);
				msg.Read(out ushort val12);
				msg.Read(out ushort val13);
				msg.Read(out int val14);
				ChannelManager.Instance.UpdateAlways(val2, val3, val4, val5, val6, val7, val8, val9, val10, val11, val12, val13, val14);
				if (val3 == 1)
				{
					BuildOption.Instance.Props.maxNewbieLevel = val11;
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("CS_CHANNEL_ACK " + ex.Message.ToString());
			}
		}
	}

	private void HandleCS_CHANNEL_END_ACK(MsgBody msg)
	{
	}

	private void HandleCS_ROAMIN_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnRoamIn", val);
		}
	}

	private void HandleCS_ROAMOUT_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnRoamOut", val);
		}
	}

	private void HandleCS_CUR_CHANNEL_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		ChannelManager.Instance.CurChannelId = val;
	}

	private void HandleCS_PLAYER_INIT_INFO_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out sbyte val2);
		msg.Read(out int val3);
		msg.Read(out sbyte val4);
		msg.Read(out int val5);
		msg.Read(out int val6);
		msg.Read(out int val7);

        MyInfoManager.Instance.InitInfo(val, val2, val3, val4, val5, val6, val7);
	}

	private void HandleCS_DUPLICATE_REPORT_ACK(MsgBody msg)
	{
		MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("DUPLICATE_REPORT"));
	}

	private void HandleCS_LEVELUP_EVENT_ACK(MsgBody msg)
	{
		msg.Read(out byte val);
		msg.Read(out int val2);
		string nick = string.Empty;
		string arg = string.Empty;
		NameCard nameCard = MyInfoManager.Instance.GetFriend(val2);
		if (nameCard != null)
		{
			nick = StringMgr.Instance.Get("SHORT_FRIEND");
			arg = nameCard.Nickname;
		}
		else
		{
			nameCard = MyInfoManager.Instance.GetClanee(val2);
			if (nameCard != null)
			{
				nick = StringMgr.Instance.Get("SHORT_CLAN");
				arg = nameCard.Nickname;
			}
		}
		GameObject gameObject = GameObject.Find("Main");
		switch (val)
		{
		case 0:
		{
			msg.Read(out int val3);
			NameCard user = ChannelUserManager.Instance.GetUser(val2);
			if (user != null)
			{
				user.Lv = XpManager.Instance.GetLevel(val3);
			}
			if (nameCard != null)
			{
				nameCard.Lv = XpManager.Instance.GetLevel(val3);
				string rank = XpManager.Instance.GetRank(nameCard.Lv, nameCard.Rank);
				if (null != gameObject)
				{
					gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, nick, string.Format(StringMgr.Instance.Get("NEWS_MESSAGE_01"), arg, rank)));
				}
			}
			break;
		}
		case 1:
			msg.Read(out string _);
			break;
		case 2:
			msg.Read(out int _);
			msg.Read(out string val7);
			if (null != gameObject)
			{
				gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, nick, string.Format(StringMgr.Instance.Get("NEWS_MESSAGE_03"), arg, val7)));
			}
			break;
		case 3:
			msg.Read(out string val4);
			if (null != gameObject)
			{
				nick = StringMgr.Instance.Get("SHORT_CLAN");
				gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, nick, string.Format(StringMgr.Instance.Get("NEWS_MESSAGE_04"), val4, MyInfoManager.Instance.ClanName)));
			}
			break;
		case 4:
			if (null != gameObject)
			{
				gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, nick, string.Format(StringMgr.Instance.Get("FRIEND_LOG_IN"), arg)));
			}
			break;
		}
	}

	private void HandleCS_GET_CANNON_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		GameObject brickObject = BrickManager.Instance.GetBrickObject(val2);
		if (null != brickObject)
		{
			CannonController componentInChildren = brickObject.GetComponentInChildren<CannonController>();
			if (null != componentInChildren)
			{
				componentInChildren.SetShooter(val);
			}
		}
	}

	private void HandleCS_GET_TRAIN_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		TrainManager.Instance.SetShooter(val2, val);
	}

	private void HandleCS_GOT_CANNON_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		GameObject brickObject = BrickManager.Instance.GetBrickObject(val2);
		if (null != brickObject)
		{
			CannonController componentInChildren = brickObject.GetComponentInChildren<CannonController>();
			if (null != componentInChildren)
			{
				componentInChildren.SetShooter(val);
			}
		}
	}

	private void HandleCS_EMPTY_CANNON_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		GameObject brickObject = BrickManager.Instance.GetBrickObject(val);
		if (null != brickObject)
		{
			CannonController componentInChildren = brickObject.GetComponentInChildren<CannonController>();
			if (componentInChildren != null)
			{
				componentInChildren.SetShooter(-1);
			}
		}
	}

	private void HandleCS_EMPTY_TRAIN_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		TrainManager.Instance.SetShooter(val, -1);
	}

	private void HandleCS_XTRAP_PACKET(MsgBody msg)
	{
		byte[] array = new byte[256];
		for (int i = 0; i < 256; i++)
		{
			msg.Read(out byte val);
			array[i] = val;
		}
		if (BuildOption.Instance.Props.UseXTrap)
		{
			byte[] array2 = new byte[256];
			WXCS_IF.XTrap_CS_Step2(array, array2, WXCS_IF.CS2_POPT_PE | WXCS_IF.CS2_POPT_TEXT | WXCS_IF.CS2_POPT_E_V);
			SendCS_XTRAP_PACKET(array2);
		}
	}

	public void HandleCS_ROUND_ROBIN_ACK(MsgBody msg)
	{
		msg.Read(out string val);
		msg.Read(out int val2);
        CSNetManager.Instance.BfServer = val;
		CSNetManager.Instance.BfPort = val2;
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnRoundRobin");
		}
	}

	private void HandleCS_SERVICE_FAIL_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnServiceFail", val);
		}
	}

	private void HandleCS_WAITING_QUEUING_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		WaitQueueDialog waitQueueDialog = null;
		if (DialogManager.Instance.IsPopup(DialogManager.DIALOG_INDEX.WAIT_QUEUE))
		{
			waitQueueDialog = (WaitQueueDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.WAIT_QUEUE);
		}
		else
		{
			waitQueueDialog = (WaitQueueDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.WAIT_QUEUE, exclusive: true);
			waitQueueDialog?.InitDialog();
		}
		if (waitQueueDialog != null)
		{
			waitQueueDialog.Waiting = val;
		}
	}

	private void HandleCS_DOWNLOADED_MAP_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out string val2);
		msg.Read(out string val3);
		msg.Read(out ushort val4);
		msg.Read(out byte val5);
		msg.Read(out byte val6);
		msg.Read(out int val7);
		msg.Read(out sbyte val8);
		msg.Read(out sbyte val9);
		msg.Read(out sbyte val10);
		msg.Read(out sbyte val11);
		msg.Read(out sbyte val12);
		msg.Read(out int val13);
		msg.Read(out int val14);
		msg.Read(out int val15);
		msg.Read(out int val16);
		msg.Read(out int val17);
		msg.Read(out int val18);
		bool clanMatchable = (val5 & Room.clanMatch) != 0;
		bool officialMap = (val5 & Room.official) != 0;
		bool blocked = (val5 & Room.blocked) != 0;
		DateTime regDate = new DateTime(val7, val8, val9, val10, val11, val12);
		RegMap always = RegMapManager.Instance.GetAlways(val, val2, val3, regDate, val4, clanMatchable, officialMap, val16, val17, val18, val13, val14, val15, val6, blocked);
		if (always != null)
		{
			RegMapManager.Instance.SetDownload(val, download: true);
		}
		else
		{
			Debug.LogError("Fail to get regmap: " + val);
		}
	}

	private void HandleCS_DOWNLOADED_MAP_LIST_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		for (int i = 0; i < val; i++)
		{
			msg.Read(out int val2);
			msg.Read(out string val3);
			msg.Read(out string val4);
			msg.Read(out ushort val5);
			msg.Read(out byte val6);
			msg.Read(out byte val7);
			msg.Read(out int val8);
			msg.Read(out sbyte val9);
			msg.Read(out sbyte val10);
			msg.Read(out sbyte val11);
			msg.Read(out sbyte val12);
			msg.Read(out sbyte val13);
			msg.Read(out int val14);
			msg.Read(out int val15);
			msg.Read(out int val16);
			msg.Read(out int val17);
			msg.Read(out int val18);
			msg.Read(out int val19);
			bool clanMatchable = (val6 & Room.clanMatch) != 0;
			bool officialMap = (val6 & Room.official) != 0;
			bool blocked = (val6 & Room.blocked) != 0;
			DateTime regDate = new DateTime(val8, val9, val10, val11, val12, val13);
			RegMap always = RegMapManager.Instance.GetAlways(val2, val3, val4, regDate, val5, clanMatchable, officialMap, val17, val18, val19, val14, val15, val16, val7, blocked);
			if (always != null)
			{
				RegMapManager.Instance.SetDownload(val2, download: true);
			}
			else
			{
				Debug.LogError("Fail to get regmap: " + val2);
			}
		}
	}

	private void HandleCS_DOWNLOAD_MAP_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out int val3);
		RegMap regMap = RegMapManager.Instance.Get(val2);
		if (regMap != null)
		{
			if (val != 0)
			{
				string msg2 = string.Format(StringMgr.Instance.Get("SAVE_FAIL"), regMap.Alias);
				switch (val)
				{
				case -3:
					msg2 = string.Format(StringMgr.Instance.Get("NOT_LATEST_VERSION"), regMap.Alias);
					break;
				case -4:
					msg2 = StringMgr.Instance.Get("NOTICE_BLOCK_MAP");
					break;
				case -1000:
					msg2 = string.Format(StringMgr.Instance.Get("SAVE_FAIL_BY_LEVEL_LIMIT"), regMap.Alias, XpManager.Instance.GetRank(val3));
					break;
				}
				MessageBoxMgr.Instance.AddMessage(msg2);
			}
			else
			{
				RegMapManager.Instance.SetDownload(val2, download: true);
				MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("SAVE_SUCCESS"), regMap.Alias));
			}
		}
		_waitingAck = false;
	}

	private void HandleCS_DEL_DOWNLOAD_MAP_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		RegMap regMap = RegMapManager.Instance.Get(val2);
		if (regMap != null)
		{
			if (val != 0)
			{
				MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("DELETE_FILE_FAIIL"), regMap.Alias));
			}
			else
			{
				RegMapManager.Instance.SetDownload(val2, download: false);
				MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("DELETE_FILE_SUCCESS"), regMap.Alias));
			}
		}
		_waitingAck = false;
	}

	private void HandleCS_MISSION_COUNT_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out int val3);
		if (val == MyInfoManager.Instance.Seq)
		{
			MyInfoManager.Instance.Mission = val2;
			MyInfoManager.Instance.Score = val3;
		}
		else
		{
			BrickManDesc desc = BrickManManager.Instance.GetDesc(val);
			if (desc != null)
			{
				desc.Mission = val2;
				desc.Score = val3;
			}
		}
	}

	private void HandleCS_ROUND_SCORE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		if (val == MyInfoManager.Instance.Seq)
		{
			MyInfoManager.Instance.Score = val2;
		}
		else
		{
			BrickManDesc desc = BrickManManager.Instance.GetDesc(val);
			if (desc != null)
			{
				desc.Score = val2;
			}
		}
	}

	private void HandleCS_ASSIST_COUNT_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out int val3);
		if (val == MyInfoManager.Instance.Seq)
		{
			MyInfoManager.Instance.Assist = val2;
			MyInfoManager.Instance.Score = val3;
		}
		else
		{
			BrickManDesc desc = BrickManManager.Instance.GetDesc(val);
			if (desc != null)
			{
				desc.Assist = val2;
				desc.Score = val3;
			}
		}
	}

	private void HandleCS_PLAYER_DETAIL_ACK(MsgBody msg)
	{
		_waitingAck = false;
		List<string> list = new List<string>();
		msg.Read(out int val);
		msg.Read(out string val2);
		msg.Read(out int val3);
		msg.Read(out int val4);
		msg.Read(out int val5);
		msg.Read(out int val6);
		msg.Read(out int val7);
		msg.Read(out int val8);
		msg.Read(out int val9);
		msg.Read(out int val10);
		msg.Read(out int val11);
		msg.Read(out int val12);
		msg.Read(out int val13);
		msg.Read(out int val14);
		msg.Read(out int val15);
		msg.Read(out int val16);
		msg.Read(out int val17);
		msg.Read(out int val18);
		msg.Read(out int val19);
		msg.Read(out int val20);
		msg.Read(out int val21);
		msg.Read(out int val22);
		msg.Read(out int val23);
		msg.Read(out int val24);
		msg.Read(out int val25);
		msg.Read(out int val26);
		msg.Read(out int val27);
		msg.Read(out int val28);
		msg.Read(out int val29);
		msg.Read(out int val30);
		msg.Read(out int val31);
		msg.Read(out int val32);
		msg.Read(out int val33);
		msg.Read(out int val34);
		msg.Read(out int val35);
		msg.Read(out int val36);
		msg.Read(out int val37);
		msg.Read(out int val38);
		msg.Read(out int val39);
		msg.Read(out int val40);
		msg.Read(out int val41);
		msg.Read(out int val42);
		msg.Read(out int val43);
		msg.Read(out int val44);
		msg.Read(out int val45);
		msg.Read(out int val46);
		msg.Read(out int val47);
		msg.Read(out int val48);
		msg.Read(out int val49);
		msg.Read(out int val50);
		msg.Read(out int val51);
		msg.Read(out int val52);
		msg.Read(out int val53);
		msg.Read(out int val54);
		msg.Read(out int val55);
		msg.Read(out int val56);
		msg.Read(out int val57);
		msg.Read(out int val58);
		msg.Read(out int val59);
		msg.Read(out int val60);
		msg.Read(out int val61);
		msg.Read(out int val62);
		msg.Read(out int val63);
		msg.Read(out int val64);
		msg.Read(out int val65);
		msg.Read(out int val66);
		msg.Read(out int val67);
		msg.Read(out int val68);
		for (int i = 0; i < val68; i++)
		{
			msg.Read(out string val69);
			list.Add(val69);
		}
		msg.Read(out int val70);
		msg.Read(out string val71);
		msg.Read(out int val72);
		msg.Read(out int val73);
		msg.Read(out int val74);
		msg.Read(out int val75);
		msg.Read(out int val76);
		msg.Read(out int val77);
		msg.Read(out int val78);
		msg.Read(out int val79);
		msg.Read(out int val80);
		((PlayerDetailDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.PLAYER_DETAIL, exclusive: false))?.InitDialog(val, val2, val3, val4, val5, val6, val7, val8, val9, val10, val11, val12, val13, val14, val15, val16, val17, val18, val19, val20, val21, val22, val23, val24, val25, val26, val27, val28, val29, val30, val31, val32, val33, val34, val35, val36, val37, val38, val39, val40, val41, val42, val43, val44, val45, val46, val47, val48, val49, val50, val51, val52, val53, val54, val55, val56, val57, val58, val59, val60, val61, val62, val63, val64, val65, val66, val67, list, val70, val71, val72, val73, val74, val75, val76, val77, val78, val79, val80);
	}

	private void HandleCS_SEND_CLAN_INVITATION_ACK(MsgBody msg)
	{
		msg.Read(out long val);
		msg.Read(out string val2);
		if (val >= 0)
		{
			MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("SUCCESS_TO_SEND_CLAN_INVITATION"), val2));
		}
		else
		{
			string msg2 = string.Format(StringMgr.Instance.Get("FAIL_TO_SEND_CLAN_INVITATION"), val2);
			switch (val)
			{
			case -1L:
				msg2 = StringMgr.Instance.Get("NO_AUTH4SEND_CLAN_INVITATION");
				break;
			case -2L:
				msg2 = string.Format(StringMgr.Instance.Get("FAIL_TO_SEND_CLAN_INVITATION_MEMO_FULL"), val2);
				break;
			}
			MessageBoxMgr.Instance.AddMessage(msg2);
		}
	}

	private void HandleCS_ANSWER_CLAN_INVITATION_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out long val2);
		msg.Read(out int _);
		msg.Read(out bool val4);
		if (val == 0)
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get((!val4) ? "SUCCESS_TO_REPLY_NO_CLAN_INVITATION" : "SUCCESS_TO_REPLY_YES_CLAN_INVITATION"));
			MemoManager.Instance.ClearPresent(val2);
		}
		else
		{
			string msg2 = StringMgr.Instance.Get("FAIL_TO_REPLY_CLAN_INVITATION");
			switch (val)
			{
			case -3:
				msg2 = StringMgr.Instance.Get("FAIL_TO_REPLY_CLAN_INVITATION_ALREADY_MEMBER");
				break;
			case -11:
				msg2 = StringMgr.Instance.Get("FAIL_TO_REPLY_CLAN_INVITATION_TOO_MANY_CLANMEMBERS");
				break;
			}
			MessageBoxMgr.Instance.AddMessage(msg2);
		}
		_waitingAck = false;
	}

	private void HandleCS_CHECK_CLAN_NAME_AVAILABILITY_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out string val2);
		msg.Read(out string val3);
		((CreateClanDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.CREATE_CLAN))?.SetClanNameAvailability(val2, val == 0);
		if (val == -32)
		{
			MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("BAD_WORD_DETECT"), val3));
		}
	}

	private void HandleCS_CREATE_CLAN_ACK(MsgBody msg)
	{
		_waitingAck = false;
		msg.Read(out int val);
		msg.Read(out string val2);
		if (val >= 0)
		{
			MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("CREATE_CLAN_SUCCESS"), val2));
			MyInfoManager.Instance.ClanSeq = val;
			MyInfoManager.Instance.ClanName = val2;
			MyInfoManager.Instance.ClanMark = -1;
			MyInfoManager.Instance.ClanLv = 2;
			((ClanDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.CLAN))?.OnEnterClan();
		}
		else
		{
			string msg2 = StringMgr.Instance.Get("FAIL_TO_CREATE_CLAN");
			switch (val)
			{
			case -5:
				msg2 = string.Format(StringMgr.Instance.Get("NAME_IS_NOT_AVAILABLE"), val2);
				break;
			case -3:
				msg2 = StringMgr.Instance.Get("CLAN_MEMBER_CANT_CREATE_CLAN");
				break;
			case -6:
				msg2 = StringMgr.Instance.Get("NOT_ENOUGH_MONEY4CREATE_CLAN");
				break;
			}
			MessageBoxMgr.Instance.AddMessage(msg2);
		}
	}

	private void HandleCS_LEAVE_CLAN_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		if (val != 0)
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("FAIL_TO_LEAVE_CLAN"));
		}
		else
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("LEAVE_CLAN_SUCCEEDED"));
			MyInfoManager.Instance.ClanSeq = -1;
			MyInfoManager.Instance.ClanName = string.Empty;
			MyInfoManager.Instance.ClanMark = -1;
			MyInfoManager.Instance.ClanLv = -1;
			MyInfoManager.Instance.ClanMaName = string.Empty;
			MyInfoManager.Instance.ClearClanee();
		}
		_waitingAck = false;
	}

	private void HandleCS_RESET_MY_CLAN_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out string val2);
		msg.Read(out int val3);
		msg.Read(out int val4);
		int clanSeq = MyInfoManager.Instance.ClanSeq;
		MyInfoManager.Instance.ClanSeq = val;
		MyInfoManager.Instance.ClanName = val2;
		MyInfoManager.Instance.ClanMark = val3;
		MyInfoManager.Instance.ClanLv = val4;
		if (val < 0)
		{
			if (clanSeq >= 0)
			{
				MyInfoManager.Instance.ClearClanee();
				GameObject gameObject = GameObject.Find("Main");
				if (null != gameObject)
				{
					gameObject.BroadcastMessage("OnClanExiled", SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		else
		{
			((ClanDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.CLAN))?.OnEnterClan();
		}
	}

	private void HandleCS_DESTROY_CLAN_ACK(MsgBody msg)
	{
		_waitingAck = false;
		msg.Read(out int val);
		msg.Read(out int _);
		if (val == 0)
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CLOSE_CLAN_SUCCESS"));
			MyInfoManager.Instance.ClanSeq = -1;
			MyInfoManager.Instance.ClanName = string.Empty;
			MyInfoManager.Instance.ClanMark = -1;
			MyInfoManager.Instance.ClanLv = -1;
			MyInfoManager.Instance.ClanMaName = string.Empty;
			MyInfoManager.Instance.ClearClanee();
		}
		else
		{
			string msg2 = StringMgr.Instance.Get("FAIL_TO_CLOSE_CLAN");
			switch (val)
			{
			case -1:
				msg2 = StringMgr.Instance.Get("NO_AUTH4CLOSE_CLAN");
				break;
			case -4:
				msg2 = StringMgr.Instance.Get("MEMBER_REMAIN4CLOSE_CLAN");
				break;
			}
			MessageBoxMgr.Instance.AddMessage(msg2);
		}
	}

	private void HandleCS_CLAN_DETAIL_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out string val2);
		msg.Read(out string val3);
		msg.Read(out int val4);
		msg.Read(out int val5);
		msg.Read(out int val6);
		msg.Read(out int val7);
		msg.Read(out int val8);
		msg.Read(out int val9);
		msg.Read(out int val10);
		msg.Read(out int val11);
		msg.Read(out int val12);
		msg.Read(out string val13);
		((ClanDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.CLAN))?.SetClanDetail(val, val2, val3, val4, val5, val6, val7, val8, val9, val10, val11, val12, val13);
	}

	private void HandleCS_CLAN_MEMBER_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out string val2);
		msg.Read(out int val3);
		msg.Read(out int val4);
		msg.Read(out int val5);
		msg.Read(out int val6);
		msg.Read(out int val7);
		((ClanDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.CLAN))?.AddClanMember(val, val2, val3, val6, val4, val5, val7);
	}

	private void HandleCS_CLAN_MEMBER_END_ACK(MsgBody msg)
	{
		ClanDialog clanDialog = (ClanDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.CLAN);
		if (clanDialog == null)
		{
		}
	}

	private void HandleCS_CLAN_APPLICANT_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out string val2);
		msg.Read(out int val3);
		msg.Read(out int val4);
		msg.Read(out int val5);
		((ClanDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.CLAN))?.AddClanApplicant(val, val2, val3, val4, val5);
	}

	private void HandleCS_CLAN_APPLICANT_END_ACK(MsgBody msg)
	{
		ClanDialog clanDialog = (ClanDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.CLAN);
		if (clanDialog == null)
		{
		}
	}

	private void HandleCS_OPEN_RANDOM_BOX_ACK(MsgBody msg)
	{
		msg.Read(out long val);
		msg.Read(out string val2);
		msg.Read(out int val3);
		if (val >= 0)
		{
			if (DialogManager.Instance.IsPopup(DialogManager.DIALOG_INDEX.RANDOMBOX))
			{
				((RandomBoxDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.RANDOMBOX))?.OpenRandomBox(val2, val3);
			}
			MyInfoManager.Instance.OpenRandomBoxItem(val, val2, val3);
		}
		else
		{
			long num = val;
			if (num >= -4 && num <= -1)
			{
				switch (num - -4)
				{
				case 3L:
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NOT_ENOUGH_BRICKPOINT4RANDOMBOX"));
					break;
				case 2L:
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("HAVEINFINITEITEM"));
					break;
				case 1L:
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("NOT_ENOUGH_MONEY"));
					break;
				case 0L:
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("FAIL2BUY_UNKNOWN_ERROR"));
					break;
				}
			}
		}
	}

	private void HandleCS_CLAN_KICK_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out string val3);
		if (val == 0)
		{
			MyInfoManager.Instance.DelClanee(val2);
			((ClanDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.CLAN))?.DelClanMember(val2);
			MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("CLAN_KICK_SUCCEEDED"), val3));
		}
		else
		{
			string msg2 = string.Format(StringMgr.Instance.Get("FAIL_TO_EXILE"), val3);
			if (val == -1 || val == -2)
			{
				msg2 = string.Format(StringMgr.Instance.Get("FAIL_TO_EXILE_NO_AUTH"), val3);
			}
			MessageBoxMgr.Instance.AddMessage(msg2);
		}
		_waitingAck = false;
	}

	private void HandleCS_UP_CLAN_MEMBER_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out string val3);
		if (val == 0)
		{
			((ClanDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.CLAN))?.UpdateClanMemberLevel(val2, 1);
			MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("PROMOTE_SUCCEEDED"), val3));
		}
		else
		{
			string msg2 = StringMgr.Instance.Get("NO_MORE_PROMOTE");
			if (val == -1)
			{
				msg2 = StringMgr.Instance.Get("NO_AUTH_PROMOOTE");
			}
			MessageBoxMgr.Instance.AddMessage(msg2);
		}
		_waitingAck = false;
	}

	private void HandleCS_DOWN_CLAN_MEMBER_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out string val3);
		if (val == 0)
		{
			((ClanDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.CLAN))?.UpdateClanMemberLevel(val2, 0);
			MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("DEMOTE_SUCCEEDED"), val3));
		}
		else
		{
			string msg2 = StringMgr.Instance.Get("NO_MORE_DEMOTE");
			if (val == -1)
			{
				msg2 = StringMgr.Instance.Get("NO_AUTH_DEMOTE");
			}
			MessageBoxMgr.Instance.AddMessage(msg2);
		}
		_waitingAck = false;
	}

	private void HandleCS_TRANSFER_MASTER_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out string val3);
		if (val == 0)
		{
			((ClanDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.CLAN))?.UpdateClanMemberLevel(val2, 2);
			MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("DELEGATE_SUCCEEDED"), val3));
		}
		else
		{
			string msg2 = StringMgr.Instance.Get("FAIL_TO_DELEGATE");
			if (val == -1)
			{
				msg2 = StringMgr.Instance.Get("NO_AUTH_DELEGATE");
			}
			MessageBoxMgr.Instance.AddMessage(msg2);
		}
		_waitingAck = false;
	}

	private void HandleCS_ACCEPT_APPLICANT_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int _);
		msg.Read(out bool val3);
		msg.Read(out string val4);
		if (val == 0)
		{
			((ClanDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.CLAN))?.DelClanApplicant(val4);
			if (val3)
			{
				MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("APPLICANT_ACCEPTED"), val4));
			}
			else
			{
				MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("APPLICANT_REFUSED"), val4));
			}
		}
		else
		{
			string msg2 = StringMgr.Instance.Get("FAIL_TO_ACCEPT_APPLICANT");
			switch (val)
			{
			case -11:
				msg2 = StringMgr.Instance.Get("FAIL_TO_ACCEPT_APPLICANT_TOO_MANY_CLANMEMBERS");
				break;
			case -3:
				msg2 = StringMgr.Instance.Get("FAIL_TO_REPLY_CLAN_INVITATION_ALREADY_MEMBER");
				break;
			case -7:
				msg2 = StringMgr.Instance.Get("FAIL_TO_FIND_APPLICANT");
				break;
			case -1:
				msg2 = StringMgr.Instance.Get("NO_AUTH_APPLICANT");
				break;
			}
			MessageBoxMgr.Instance.AddMessage(msg2);
		}
		_waitingAck = false;
	}

	private void HandleCS_APPLY_CLAN_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out string val2);
		if (val == 0)
		{
			MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("APPLY_CLAN_SUCCEEDED"), val2));
		}
		else
		{
			string msg2 = string.Format(StringMgr.Instance.Get("FAIL_TO_APPLY_CLAN"), val2);
			if (val == -2)
			{
				msg2 = StringMgr.Instance.Get("WARNING_MSG_CLAN_JOIN");
			}
			switch (val)
			{
			case -3:
				msg2 = StringMgr.Instance.Get("CLAN_MEMBER_CANT_APPLY_CLAN");
				break;
			case -100:
				msg2 = StringMgr.Instance.Get("ALREADY_CLAN_APPLY");
				break;
			}
			MessageBoxMgr.Instance.AddMessage(msg2);
		}
		_waitingAck = false;
	}

	private void HandleCS_CHANGE_CLAN_INTRO_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out string val2);
		if (val != 0)
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("FAIL_TO_CHANGE_CLAN_INTRO"));
		}
		else
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CHANGE_CLAN_INTRO_SUCCEEDED"));
			ClanDialog clanDialog = (ClanDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.CLAN);
			if (clanDialog != null)
			{
				clanDialog.MyClanIntro = val2;
			}
		}
	}

	private void HandleCS_CHANGE_CLAN_NOTICE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out string val2);
		if (val != 0)
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("FAIL_TO_CHANGE_CLAN_NOTICE"));
		}
		else
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CHANGE_CLAN_NOTICE_SUCCEEDED"));
			ClanDialog clanDialog = (ClanDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.CLAN);
			if (clanDialog != null)
			{
				clanDialog.MyClanNotice = val2;
			}
		}
	}

	private void HandleCS_CLAN_INFO_CHANGE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		if (val != MyInfoManager.Instance.Seq)
		{
			SendCS_CLAN_MEMBER_REQ(val, val2);
		}
	}

	private void HandleCS_CLAN_NEW_MEMBER_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		if (val != MyInfoManager.Instance.Seq)
		{
			SendCS_CLAN_MEMBER_REQ(val, val2);
		}
	}

	private void HandleCS_CLAN_DEL_MEMBER_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int _);
		if (val != MyInfoManager.Instance.Seq)
		{
			MyInfoManager.Instance.DelClanee(val);
			((ClanDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.CLAN))?.DelClanMember(val);
		}
	}

	private void HandleCS_CLAN_MARK_CHANGED_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		MyInfoManager.Instance.ClanMark = val;
	}

	private void HandleCS_CHANGE_CLAN_MARK_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		string msg2 = StringMgr.Instance.Get("FAIL_TO_CHANGE_CLAN_MARK");
		switch (val)
		{
		case -9:
			msg2 = StringMgr.Instance.Get("CLAN_MARK_IS_USING");
			break;
		case -10:
		{
			TItem specialItem2HaveFunction = TItemManager.Instance.GetSpecialItem2HaveFunction("clan_mark");
			if (specialItem2HaveFunction != null)
			{
				string msg3 = string.Format(StringMgr.Instance.Get("CLAN_MARK_TICKET_NEED"), specialItem2HaveFunction.Name);
				MessageBoxMgr.Instance.AddMessage(msg3);
			}
			break;
		}
		case 0:
		{
			msg2 = StringMgr.Instance.Get("CHANGE_CLAN_MARK_SUCCEEDED");
			MarkDialog markDialog = (MarkDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.MARK);
			if (markDialog != null)
			{
				markDialog.CurMark = val2;
			}
			break;
		}
		}
		MessageBoxMgr.Instance.AddMessage(msg2);
		_waitingAck = false;
	}

	private void HandleCS_CREATE_SQUAD_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int _);
		if (val < 0)
		{
			GlobalVars.Instance.clanSendSqudREQ = -1;
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("FAIL_TO_CREATE_SQUAD"));
		}
		else
		{
			GlobalVars.Instance.clanSendSqudREQ = 2;
			SquadManager.Instance.Join(val);
		}
		_waitingAck = false;
	}

	private void HandleCS_JOIN_SQUAD_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int _);
		if (val < 0)
		{
			GlobalVars.Instance.clanSendJoinREQ = -1;
			if (val == -2)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CLAN_MATCH_TEAM_MATCHING"));
			}
			else
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("FAIL_TO_JOIN_SQUAD"));
			}
		}
		else
		{
			SquadManager.Instance.Join(val);
			if (GlobalVars.Instance.clanSendJoinREQ >= 0)
			{
				GlobalVars.Instance.clanSendJoinREQ = 2;
			}
			GlobalVars.Instance.clanTeamMatchSuccess = 1;
		}
		_waitingAck = false;
	}

	private void HandleCS_ENTER_SQUAD_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out string val2);
		msg.Read(out int val3);
		msg.Read(out int val4);
		SquadManager.Instance.AddMember(val, val2, val3, val4);
	}

	private void HandleCS_LEAVE_SQUAD_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		if (val != MyInfoManager.Instance.Seq)
		{
			SquadManager.Instance.DelMember(val);
		}
	}

	private void HandleCS_ADD_SQUAD_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out int val3);
		msg.Read(out int val4);
		msg.Read(out int val5);
		msg.Read(out int val6);
		msg.Read(out int val7);
		msg.Read(out int val8);
		NameCard nameCard = ChannelUserManager.Instance.GetUser(val5);
		if (nameCard == null)
		{
			nameCard = MyInfoManager.Instance.GetClanee(val5);
		}
		string leaderNickname = string.Empty;
		if (nameCard != null)
		{
			leaderNickname = nameCard.Nickname;
		}
		if (val5 == MyInfoManager.Instance.Seq)
		{
			leaderNickname = MyInfoManager.Instance.Nickname;
		}
		SquadManager.Instance.UpdateAlways(val, val2, val3, val4, val6, val7, val8, val5, leaderNickname);
	}

	private void HandleCS_DEL_SQUAD_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		SquadManager.Instance.Del(val, val2);
	}

	private void HandleCS_UPDATE_SQUAD_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out int val3);
		msg.Read(out int val4);
		msg.Read(out int val5);
		msg.Read(out int val6);
		msg.Read(out int val7);
		msg.Read(out int val8);
		NameCard nameCard = ChannelUserManager.Instance.GetUser(val5);
		if (nameCard == null)
		{
			nameCard = MyInfoManager.Instance.GetClanee(val5);
		}
		string leaderNickname = string.Empty;
		if (nameCard != null)
		{
			leaderNickname = nameCard.Nickname;
		}
		if (val5 == MyInfoManager.Instance.Seq)
		{
			leaderNickname = MyInfoManager.Instance.Nickname;
		}
		SquadManager.Instance.UpdateAlways(val, val2, val3, val4, val6, val7, val8, val5, leaderNickname);
	}

	private void HandleCS_ENTER_SQUADING_ACK(MsgBody msg)
	{
		_waitingAck = false;
		msg.Read(out int val);
		if (val == 0)
		{
			if (ChannelManager.Instance.CurChannel.Mode == 4)
			{
				if (GlobalVars.Instance.clanSendSqudREQ == 0)
				{
					CSNetManager.Instance.Sock.SendCS_CREATE_SQUAD_REQ(MyInfoManager.Instance.ClanSeq, GlobalVars.Instance.wannaPlayMap, GlobalVars.Instance.wannaPlayMode, GlobalVars.Instance.clanMatchMaxPlayer);
				}
				else if (GlobalVars.Instance.clanSendSqudREQ == 1 || GlobalVars.Instance.clanSendJoinREQ == 0)
				{
					Room room = RoomManager.Instance.GetRoom(GlobalVars.Instance.roomNo);
					if (room != null)
					{
						CSNetManager.Instance.Sock.SendCS_JOIN_SQUAD_REQ(MyInfoManager.Instance.ClanSeq, room.Squad, room.SquadCounter);
					}
				}
				else if (GlobalVars.Instance.clanSendJoinREQ == 1)
				{
					CSNetManager.Instance.Sock.SendCS_JOIN_SQUAD_REQ(InviteManager.Instance.GetData().clanSeq, InviteManager.Instance.GetData().squadIndex, InviteManager.Instance.GetData().squadCounterIndex);
				}
			}
		}
		else
		{
			GlobalVars.Instance.clanSendSqudREQ = -1;
			GlobalVars.Instance.clanSendJoinREQ = -1;
			int num = val;
			if (num == -2)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CLAN_MATCH_ONLY_FOR_CLAN_MEMBER"));
			}
		}
	}

	private void HandleCS_LEAVE_SQUADING_ACK(MsgBody msg)
	{
	}

	private void HandleCS_KICK_SQUAD_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		string empty = string.Empty;
		if (val == MyInfoManager.Instance.Seq)
		{
			_waitingAck = false;
			DialogManager.Instance.CloseAll();
			ContextMenuManager.Instance.CloseAll();
			string msg2 = string.Format(StringMgr.Instance.Get("HAS_BEEN_KICKED"), MyInfoManager.Instance.Nickname);
			MessageBoxMgr.Instance.AddMessage(msg2);
			SquadManager.Instance.Leave();
			SendCS_MAKE_SQUAD_NULL_REQ();
			Application.LoadLevel("Squading");
		}
		else
		{
			NameCard nameCard = SquadManager.Instance.GetSquadMember(val);
			if (nameCard == null)
			{
				nameCard = ChannelUserManager.Instance.GetUser(val);
			}
			if (nameCard == null)
			{
				nameCard = MyInfoManager.Instance.GetClanee(val);
			}
			if (nameCard != null)
			{
				empty = nameCard.Nickname;
				string msg3 = string.Format(StringMgr.Instance.Get("HAS_BEEN_KICKED"), empty);
				MessageBoxMgr.Instance.AddMessage(msg3);
			}
		}
	}

	private void HandleCS_CHG_SQUAD_OPTION_ACK(MsgBody msg)
	{
		msg.Read(out int _);
		msg.Read(out int val2);
		msg.Read(out int val3);
		msg.Read(out int val4);
		msg.Read(out int val5);
		Squad squad = SquadManager.Instance.GetSquad(val2);
		if (squad != null)
		{
			squad.WannaPlayMap = val3;
			squad.WannaPlayMode = val4;
			squad.MaxMember = val5;
		}
	}

	private void HandleCS_CLAN_MATCH_HALF_TIME_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnClanMatchHalfTime", val);
		}
	}

	private void HandleCS_BND_SHIFT_PHASE_ACK(MsgBody msg)
	{
		msg.Read(out int repeat);
		msg.Read(out bool isBuildPhase);
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
            Debug.LogWarning(repeat);
			gameObject.BroadcastMessage("OnRoundEnd", repeat);
			if (RoomManager.Instance.Master != MyInfoManager.Instance.Seq)
			{
				BndTimer component = gameObject.GetComponent<BndTimer>();
				if (null != component)
				{
                    Debug.LogWarning("Shift Phase not host");
					component.ShiftPhase(isBuildPhase);
				}
			}
		}
		if (MyInfoManager.Instance.BndModeDesc != null)
		{
			MyInfoManager.Instance.BndModeDesc.buildPhase = isBuildPhase;
		}
	}

	private void HandleCS_NOTICE_ACK(MsgBody msg)
	{
		msg.Read(out string val);
		msg.Read(out sbyte val2);
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			if ((val2 & 1) != 0 && MyInfoManager.Instance.Status != 4)
			{
				val = RemoveSystemKey(val);
				gameObject.BroadcastMessage("OnNotice", val, SendMessageOptions.DontRequireReceiver);
			}
			if ((val2 & 2) != 0)
			{
				val = RemoveSystemKey(val);
				gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, 0, string.Empty, val));
			}
			if ((val2 & 4) != 0)
			{
				gameObject.BroadcastMessage("OnNoticeCenter", val, SendMessageOptions.DontRequireReceiver);
			}
		}
		GameObject gameObject2 = GameObject.Find("Me");
		if (null != gameObject2)
		{
			LocalController component = gameObject2.GetComponent<LocalController>();
			if (component != null && (val2 & 4) != 0)
			{
				gameObject2.BroadcastMessage("OnNoticeCenter", val, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private void HandleCS_AD_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		for (int i = 0; i < val; i++)
		{
			msg.Read(out int val2);
			msg.Read(out string val3);
			msg.Read(out int val4);
			msg.Read(out string val5);
			BannerManager.Instance.AddAd(val2, val3, val4, val5);
		}
	}

	private void HandleCS_ROUND_END_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out sbyte val2);
		msg.Read(out sbyte val3);
		msg.Read(out sbyte val4);
		if (RoomManager.Instance.CurrentRoomType != Room.ROOM_TYPE.CAPTURE_THE_FLAG)
		{
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				gameObject.BroadcastMessage("OnRoundEnd", val);
				ExplosionMatch component = gameObject.GetComponent<ExplosionMatch>();
				if (null != component)
				{
					component.SetRoundResult(val2, val3, val4);
				}
				ZombieMatch component2 = gameObject.GetComponent<ZombieMatch>();
				if (null != component2)
				{
					component2.SetRoundResult(val2, val3, val4);
				}
			}
		}
	}

	private void HandleCS_GET_BACK2SPAWNER_ACK(MsgBody msg)
	{
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnGetBack2Spawner", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void HandleCS_MATCH_RESTART_COUNT_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnMatchRestartCount", val);
		}
	}

	private void HandleCS_MATCH_RESTARTED_ACK(MsgBody msg)
	{
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnMatchRestarted");
		}
	}

	private void HandleCS_CLAN_MATCH_RECORD_LIST_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		ClanDialog clanDialog = (ClanDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.CLAN);
		if (clanDialog != null)
		{
			if (val2 > 0)
			{
				clanDialog.AddClanMatchStart(val);
			}
			else if (val == 1 && val2 <= 0)
			{
				clanDialog.ThisClanHasNoClanMatchRecord();
			}
		}
		for (int i = 0; i < val2; i++)
		{
			msg.Read(out long val3);
			msg.Read(out int val4);
			msg.Read(out int val5);
			msg.Read(out int val6);
			msg.Read(out int val7);
			msg.Read(out string val8);
			msg.Read(out int val9);
			msg.Read(out int val10);
			msg.Read(out int val11);
			msg.Read(out int val12);
			msg.Read(out int val13);
			msg.Read(out int val14);
			msg.Read(out int val15);
			msg.Read(out int val16);
			clanDialog?.AddClanMatch(val3, val4, val5, val6, val7, val8, val9, val10, val11, val12, val13, val14, val15, val16);
		}
		if (clanDialog != null && val2 > 0)
		{
			clanDialog.AddClanMatchEnd();
		}
	}

	private void HandleCS_CLAN_MATCH_PLAYER_LIST_ACK(MsgBody msg)
	{
		ClanDialog clanDialog = (ClanDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.CLAN);
		msg.Read(out long val);
		msg.Read(out int val2);
		for (int i = 0; i < val2; i++)
		{
			msg.Read(out int val3);
			msg.Read(out int val4);
			msg.Read(out int val5);
			msg.Read(out string val6);
			msg.Read(out int val7);
			msg.Read(out int val8);
			msg.Read(out int val9);
			msg.Read(out int val10);
			clanDialog?.AddClanMatchPlayer(val, val3, val4, val5, val6, val7, val8, val9, val10);
		}
		clanDialog?.AddClanMatchPlayerEnd(val);
	}

	private void HandleCS_MATCH_TEAM_START_ACK(MsgBody msg)
	{
		SquadManager.Instance.IsMatching = true;
	}

	private void HandleCS_MATCH_TEAM_CANCELED_ACK(MsgBody msg)
	{
		SquadManager.Instance.IsMatching = false;
	}

	private void HandleCS_MATCH_TEAM_CANCEL_ACK(MsgBody msg)
	{
		SquadManager.Instance.IsMatching = false;
	}

	private void HandleCS_MATCH_TEAM_SUCCESS_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		SquadManager.Instance.IsMatching = false;
		GlobalVars.Instance.clanTeamMatchSuccess = 1;
		Room room = RoomManager.Instance.GetRoom(val);
		if (room != null)
		{
			RoomManager.Instance.CurrentRoom = val;
			if (RoomManager.Instance.Master == MyInfoManager.Instance.Seq)
			{
				CSNetManager.Instance.Sock.SendCS_RESUME_ROOM_REQ(0);
			}
		}
	}

	private void HandleCS_CLAN_MATCH_TEAM_GETBACK_ACK(MsgBody msg)
	{
		DialogManager.Instance.CloseAll();
		ContextMenuManager.Instance.CloseAll();
		P2PManager.Instance.Shutdown();
		SendCS_LEAVE_REQ();
		Application.LoadLevel("Lobby");
	}

	private void HandleCS_BM_INSTALL_BOMB_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out float val3);
		msg.Read(out float val4);
		msg.Read(out float val5);
		msg.Read(out float val6);
		msg.Read(out float val7);
		msg.Read(out float val8);
		if (MyInfoManager.Instance.BlastModeDesc != null)
		{
			MyInfoManager.Instance.BlastModeDesc.rounding = false;
			MyInfoManager.Instance.BlastModeDesc.bombInstaller = val;
			MyInfoManager.Instance.BlastModeDesc.blastTarget = val2;
			MyInfoManager.Instance.BlastModeDesc.point = new Vector3(val3, val4, val5);
			MyInfoManager.Instance.BlastModeDesc.normal = new Vector3(val6, val7, val8);
		}
		else
		{
			GameObject gameObject = GameObject.Find("InstalledClockBomb");
			if (null != gameObject)
			{
				InstalledBomb component = gameObject.GetComponent<InstalledBomb>();
				if (null != component)
				{
					component.Install(new Vector3(val3, val4, val5), new Vector3(val6, val7, val8));
				}
			}
			GameObject gameObject2 = GameObject.Find("Main");
			if (null != gameObject2)
			{
				ExplosionMatch component2 = gameObject2.GetComponent<ExplosionMatch>();
				if (component2 != null)
				{
					component2.Installed(val, val2);
				}
			}
		}
	}

	private void HandleCS_BM_UNINSTALL_BOMB_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int _);
		if (MyInfoManager.Instance.BlastModeDesc != null)
		{
			MyInfoManager.Instance.BlastModeDesc.rounding = true;
			MyInfoManager.Instance.BlastModeDesc.bombInstaller = -1;
			MyInfoManager.Instance.BlastModeDesc.blastTarget = -1;
		}
		else
		{
			GameObject gameObject = GameObject.Find("InstalledClockBomb");
			if (null != gameObject)
			{
				InstalledBomb component = gameObject.GetComponent<InstalledBomb>();
				if (null != component)
				{
					component.Uninstall();
				}
			}
			GameObject gameObject2 = GameObject.Find("Main");
			if (null != gameObject2)
			{
				ExplosionMatch component2 = gameObject2.GetComponent<ExplosionMatch>();
				if (component2 != null)
				{
					component2.Uninstalled(val);
				}
			}
		}
	}

	private void HandleCS_CTF_STATUS_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int _);
		msg.Read(out float val3);
		msg.Read(out float val4);
		msg.Read(out float val5);
		BrickManManager.Instance.haveFlagSeq = val;
		BrickManManager.Instance.vFlag = new Vector3(val3, val4, val5);
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnDroped");
		}
	}

	private void HandleCS_BM_STATUS_ACK(MsgBody msg)
	{
		MyInfoManager.Instance.BlastModeDesc = new ExplosionMatchDesc();
		msg.Read(out bool val);
		MyInfoManager.Instance.BlastModeDesc.rounding = val;
		msg.Read(out int val2);
		MyInfoManager.Instance.BlastModeDesc.bombInstaller = val2;
		msg.Read(out int val3);
		MyInfoManager.Instance.BlastModeDesc.blastTarget = val3;
		msg.Read(out float val4);
		msg.Read(out float val5);
		msg.Read(out float val6);
		MyInfoManager.Instance.BlastModeDesc.point = new Vector3(val4, val5, val6);
		msg.Read(out val4);
		msg.Read(out val5);
		msg.Read(out val6);
		MyInfoManager.Instance.BlastModeDesc.normal = new Vector3(val4, val5, val6);
	}

	private void HandleCS_BM_BLAST_ACK(MsgBody msg)
	{
		msg.Read(out int _);
		msg.Read(out int _);
		if (MyInfoManager.Instance.BlastModeDesc != null)
		{
			MyInfoManager.Instance.BlastModeDesc.rounding = true;
			MyInfoManager.Instance.BlastModeDesc.bombInstaller = -1;
			MyInfoManager.Instance.BlastModeDesc.blastTarget = -1;
		}
		else
		{
			GameObject gameObject = GameObject.Find("InstalledClockBomb");
			if (null != gameObject)
			{
				InstalledBomb component = gameObject.GetComponent<InstalledBomb>();
				if (null != component)
				{
					component.Blast();
					GameObject gameObject2 = GameObject.Find("Me");
					if (null != gameObject2 && !MyInfoManager.Instance.IsSpectator && Vector3.Distance(component.transform.position, gameObject2.transform.position) < 15f)
					{
						LocalController component2 = gameObject2.GetComponent<LocalController>();
						if (null != component2)
						{
							component2.GetHit(MyInfoManager.Instance.Seq, 1000, 0f, 32, -1, autoHealPossible: false, checkZombie: false);
						}
					}
				}
			}
			GameObject gameObject3 = GameObject.Find("Main");
			if (null != gameObject3)
			{
				ExplosionMatch component3 = gameObject3.GetComponent<ExplosionMatch>();
				if (component3 != null)
				{
					component3.Blasted();
				}
			}
		}
	}

	private void HandleCS_CTF_PICK_FLAG_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int _);
		BrickManManager.Instance.haveFlagSeq = val;
		BrickManManager.Instance.bSendTcpCheckOnce = false;
		GameObject gameObject = GameObject.Find("Main");

		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnPicked", val);
		}
	}

	private void HandleCS_CTF_CAPTURE_FLAG_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int _);
		msg.Read(out bool _);
		BrickManManager.Instance.haveFlagSeq = val;
		BrickManManager.Instance.bSuccessFlagCapture = true;
		BrickManManager.Instance.bSendTcpCheckOnce = false;
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnCaptured");
		}
	}

	private void HandleCS_CTF_DROP_FLAG_ACK(MsgBody msg)
	{
		msg.Read(out int _);
		msg.Read(out int _);
		msg.Read(out float val3);
		msg.Read(out float val4);
		msg.Read(out float val5);
		BrickManManager.Instance.haveFlagSeq = -2;
		BrickManManager.Instance.bSendTcpCheckOnce = false;
		BrickManManager.Instance.vFlag = new Vector3(val3, val4, val5);
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnDroped");
		}
	}

	private void HandleCS_BLAST_MODE_SCORE_ACK(MsgBody msg)
	{
		msg.Read(out int _);
		msg.Read(out int val2);
		msg.Read(out int val3);
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnTeamScore", new TeamScore(val2, val3));
		}
	}

	private void HandleCS_CTF_SCORE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnTeamScore", new TeamScore(val, val2));
		}
	}

	private void HandleCS_CLAN_LIST_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		ClanDialog clanDialog = (ClanDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.CLAN);
		if (clanDialog != null && val2 > 0)
		{
			clanDialog.ResetClanListPage(val);
		}
		for (int i = 0; i < val2; i++)
		{
			msg.Read(out int val3);
			msg.Read(out string val4);
			msg.Read(out int val5);
			msg.Read(out int val6);
			msg.Read(out int val7);
			msg.Read(out int val8);
			msg.Read(out int val9);
			msg.Read(out int val10);
			msg.Read(out int val11);
			msg.Read(out int val12);
			msg.Read(out int val13);
			msg.Read(out int val14);
			msg.Read(out int val15);
			msg.Read(out int val16);
			msg.Read(out int val17);
			msg.Read(out int val18);
			msg.Read(out string val19);
			clanDialog?.AddClan(val3, val5, val4, val6, val7, val8, val9, val10, val11, val12, val13, val14, val15, val16, val17, val18, val19);
		}
		clanDialog?.ResetCurClan();
	}

	private void HandleCS_SELECT_CLAN_INTRO_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out string val2);
		((ClanDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.CLAN))?.UpdateCurClanIntro(val, val2);
	}

	private byte GetSendKey()
	{
		byte result = byte.MaxValue;
		if (sendKeys != null && sendKeys.Length > 0)
		{
			sendKeyIndex %= sendKeys.Length;
			result = sendKeys[sendKeyIndex];
		}
		sendKeyIndex++;
		return result;
	}

	private void HandleCS_ME_EDITOR_LIST_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		for (int i = 0; i < val; i++)
		{
			msg.Read(out int val2);
			if (MyInfoManager.Instance.Seq == val2)
			{
				MyInfoManager.Instance.IsEditor = true;
			}
			else
			{
				BrickManDesc desc = BrickManManager.Instance.GetDesc(val2);
				if (desc != null)
				{
					desc.IsEditor = true;
				}
			}
		}
	}

	private void HandleCS_ME_CHG_EDITOR_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out bool val2);
		if (MyInfoManager.Instance.Seq == val)
		{
			MyInfoManager.Instance.IsEditor = val2;
			if (val2)
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("GET_MAPEIDT_AUTH"));
			}
			else
			{
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("LOST_MAPEDIT_AUTH"));
			}
		}
		else
		{
			BrickManDesc desc = BrickManManager.Instance.GetDesc(val);
			if (desc != null)
			{
				desc.IsEditor = val2;
			}
		}
	}

	private void HandleCS_INIT_TERM_ITEM_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out long val2);
		Item itemBySequence = MyInfoManager.Instance.GetItemBySequence(val2);
		if (itemBySequence != null)
		{
			if (val == 0)
			{
				SystemMsgManager.Instance.ShowMessage(string.Format(StringMgr.Instance.Get("INIT_ITEM_SUCCESS"), itemBySequence.Template.Name));
			}
			else
			{
				MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("FAIL_TO_INIT_ITEM"), itemBySequence.Template.Name));
			}
		}
	}

	private void HandleCS_PLAYER_OPT_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out int val3);
		MyInfoManager.Instance.qjModeMask = val;
		MyInfoManager.Instance.qjOfficialMask = val2;
		MyInfoManager.Instance.qjCommonMask = val3;
	}

	private void HandleCS_WEAPON_SLOT_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out long val2);
		if (MyInfoManager.Instance.WeaponSlots.Length > val)
		{
			if (val2 < 0)
			{
				MyInfoManager.Instance.WeaponSlots[val] = -1L;
			}
			else
			{
				Item itemBySequence = MyInfoManager.Instance.GetItemBySequence(val2);
				if (itemBySequence != null && itemBySequence.Template != null && itemBySequence.Template.type == TItem.TYPE.WEAPON)
				{
					MyInfoManager.Instance.WeaponSlots[val] = val2;
				}
			}
		}
	}

	private void HandleCS_WEAPON_SLOT_LIST_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		for (int i = 0; i < val; i++)
		{
			msg.Read(out int val2);
			msg.Read(out long val3);
			if (MyInfoManager.Instance.WeaponSlots.Length > val2)
			{
				if (val3 < 0)
				{
					MyInfoManager.Instance.WeaponSlots[val2] = -1L;
				}
				else
				{
					Item itemBySequence = MyInfoManager.Instance.GetItemBySequence(val3);
					if (itemBySequence != null && itemBySequence.Template != null && itemBySequence.Template.type == TItem.TYPE.WEAPON)
					{
						MyInfoManager.Instance.WeaponSlots[val2] = val3;
					}
				}
			}
		}
	}

	private void HandleCS_SHOOTER_TOOL_ACK(MsgBody msg)
	{
		msg.Read(out sbyte val);
		msg.Read(out long val2);
		if (MyInfoManager.Instance.ShooterTools.Length > val)
		{
			if (val2 < 0)
			{
				MyInfoManager.Instance.ShooterTools[val] = -1L;
			}
			else
			{
				Item itemBySequence = MyInfoManager.Instance.GetItemBySequence(val2);
				itemBySequence.toolSlot = val;
				if (itemBySequence != null && itemBySequence.Template != null && itemBySequence.Template.type == TItem.TYPE.SPECIAL)
				{
					MyInfoManager.Instance.ShooterTools[val] = val2;
				}
			}
		}
	}

	private void HandleCS_SHOOTER_TOOL_LIST_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		for (int i = 0; i < val; i++)
		{
			msg.Read(out sbyte val2);
			msg.Read(out long val3);
			if (MyInfoManager.Instance.ShooterTools.Length > val2)
			{
				if (val3 < 0)
				{
					MyInfoManager.Instance.ShooterTools[val2] = -1L;
				}
				else
				{
					Item itemBySequence = MyInfoManager.Instance.GetItemBySequence(val3);
					if (itemBySequence != null && itemBySequence.Template != null && itemBySequence.Template.type == TItem.TYPE.SPECIAL)
					{
						MyInfoManager.Instance.ShooterTools[val2] = val3;
					}
				}
			}
		}
	}

	private void HandleCS_CTF_FLAG_RETURN_ACK(MsgBody msg)
	{
		BrickManManager.Instance.haveFlagSeq = -1;
		BrickManManager.Instance.bSendTcpCheckOnce = false;
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnReturnBack");
		}
	}

	public void HandleCS_CORE_HP_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		if (MyInfoManager.Instance.Seq != RoomManager.Instance.Master)
		{
			DefenseManager.Instance.CoreLifeRed = val;
			DefenseManager.Instance.CoreLifeBlue = val2;
		}
	}

	private void HandleCS_TC_OPEN_ACK(MsgBody msg)
	{
		_waitingAck = false;
		msg.Read(out int val);
		for (int i = 0; i < val; i++)
		{
			msg.Read(out int val2);
			msg.Read(out int val3);
			msg.Read(out int val4);
			msg.Read(out int val5);
			msg.Read(out int val6);
			msg.Read(out int val7);
			msg.Read(out int val8);
			msg.Read(out int val9);
			msg.Read(out string val10);
			msg.Read(out int val11);
			TreasureChestManager.Instance.UpdateAlways(val2, val3, val4, val5, val6, val7, val8, val9, val10);
			TcStatus tcStatus = TreasureChestManager.Instance.Get(val2);
			tcStatus.ClearExpectations();
			for (int j = 0; j < val11; j++)
			{
				msg.Read(out string val12);
				msg.Read(out int val13);
				for (int k = 0; k < val13; k++)
				{
					TcTItem item = default(TcTItem);
					item.code = val12;
					msg.Read(out item.opt);
					msg.Read(out sbyte val14);
					item.isKey = (val14 == 1);
					tcStatus.AddExpectations(item);
				}
			}
		}
		((TCGateDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.TCGATE, exclusive: true))?.InitDialog();
	}

	private void HandleCS_TC_ENTER_ACK(MsgBody msg)
	{
		_waitingAck = false;
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out int val3);
		List<byte> list = new List<byte>();
		for (int i = 0; i < val3; i++)
		{
			msg.Read(out byte val4);
			list.Add(val4);
		}
		List<int> list2 = new List<int>();
		msg.Read(out int val5);
		for (int j = 0; j < val5; j++)
		{
			msg.Read(out int val6);
			list2.Add(val6);
		}
		if (val == 0)
		{
			TcStatus tcStatus = TreasureChestManager.Instance.Get(val2);
			if (tcStatus != null)
			{
				byte[] array = new byte[8]
				{
					128,
					64,
					32,
					16,
					8,
					4,
					2,
					1
				};
				List<byte> list3 = new List<byte>();
				for (int k = 0; k < tcStatus.Max; k++)
				{
					int index = k / 8;
					int num = k % 8;
					list3.Add((byte)(((list[index] & array[num]) > 0) ? 1 : 0));
				}
				if (BuildOption.Instance.Props.randomBox == BuildOption.RANDOM_BOX_TYPE.INFERNUM)
				{
					((TCBoardDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.TCBOARD, exclusive: true))?.InitDialog(tcStatus, list3, list2);
				}
				else if (BuildOption.Instance.Props.randomBox == BuildOption.RANDOM_BOX_TYPE.NETMARBLE)
				{
					((TCNetmarbleDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.TCNETMARBLE, exclusive: true))?.InitDialog(tcStatus);
				}
			}
		}
	}

	private void HandleCS_TC_CHEST_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out int val3);
		msg.Read(out int val4);
		msg.Read(out int val5);
		msg.Read(out int val6);
		msg.Read(out int val7);
		msg.Read(out int val8);
		msg.Read(out string val9);
		msg.Read(out int val10);
		TreasureChestManager.Instance.UpdateAlways(val, val2, val3, val4, val5, val6, val7, val8, val9);
		TcStatus tcStatus = TreasureChestManager.Instance.Get(val);
		tcStatus.ClearExpectations();
		for (int i = 0; i < val10; i++)
		{
			msg.Read(out string val11);
			msg.Read(out int val12);
			for (int j = 0; j < val12; j++)
			{
				TcTItem item = default(TcTItem);
				item.code = val11;
				msg.Read(out item.opt);
				msg.Read(out sbyte val13);
				item.isKey = (val13 == 1);
				tcStatus.AddExpectations(item);
			}
		}
		if (val3 == val4)
		{
			Dialog top = DialogManager.Instance.GetTop();
			if (top != null && top.ID == DialogManager.DIALOG_INDEX.TCBOARD)
			{
				TCBoardDialog tCBoardDialog = (TCBoardDialog)top;
				tCBoardDialog.Reset(val);
			}
		}
	}

	private void HandleCS_TC_OPEN_PRIZE_TAG_ACK(MsgBody msg)
	{
		msg.Read(out long val);
		msg.Read(out int _);
		msg.Read(out int val3);
		msg.Read(out int val4);
		msg.Read(out bool val5);
		msg.Read(out bool val6);
		if (val >= 0)
		{
			SendCS_TC_RECEIVE_PRIZE_REQ(val, val3, val4, val5, val6);
		}
		else
		{
			long num = val;
			if (num >= -5 && num <= -1)
			{
				switch (num - -5)
				{
				case 4L:
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("TC_NO_SUCH_PRIZE"));
					break;
				case 3L:
					MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("TC_NOT_ENOUGH_MONEY"), TokenManager.Instance.GetTokenString()));
					break;
				case 2L:
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("TC_UNKNOWN_ERROR"));
					break;
				case 1L:
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("TC_NOT_ENOUGH_COIN"));
					break;
				case 0L:
					MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("TC_SOMEBODY_TOOKOFF"));
					break;
				}
			}
		}
	}

	private void HandleCS_TC_TOOKOFF_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out bool val3);
		TCBoardDialog tCBoardDialog = (TCBoardDialog)DialogManager.Instance.GetDialogAlways(DialogManager.DIALOG_INDEX.TCBOARD);
		if (DialogManager.Instance.IsPopup(DialogManager.DIALOG_INDEX.TCBOARD))
		{
			tCBoardDialog?.TookOff(val, val2, val3);
		}
	}

	private void HandleCS_TC_UPDATE_CHEST_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out int val3);
		msg.Read(out int val4);
		TreasureChestManager.Instance.Refresh(val, val2, val3, val4);
	}

	private void HandleCS_TC_RECEIVE_PRIZE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out long val2);
		msg.Read(out string val3);
		msg.Read(out sbyte val4);
		msg.Read(out int val5);
		msg.Read(out int val6);
		msg.Read(out int val7);
		msg.Read(out bool val8);
		msg.Read(out int val9);
		if (val >= 0)
		{
			MyInfoManager.Instance.ReceivePrize(val2, val3, (Item.USAGE)val4, val5, val9);
			if (BuildOption.Instance.Props.randomBox == BuildOption.RANDOM_BOX_TYPE.INFERNUM)
			{
				TCBoardDialog tCBoardDialog = (TCBoardDialog)DialogManager.Instance.GetDialogAlways(DialogManager.DIALOG_INDEX.TCBOARD);
				if (DialogManager.Instance.IsPopup(DialogManager.DIALOG_INDEX.TCBOARD))
				{
					tCBoardDialog?.ReceivePrize(val2, val6, val3, val7, val8);
				}
			}
			else if (BuildOption.Instance.Props.randomBox == BuildOption.RANDOM_BOX_TYPE.NETMARBLE)
			{
				TCNetmarbleDialog tCNetmarbleDialog = (TCNetmarbleDialog)DialogManager.Instance.GetDialogAlways(DialogManager.DIALOG_INDEX.TCNETMARBLE);
				if (DialogManager.Instance.IsPopup(DialogManager.DIALOG_INDEX.TCNETMARBLE))
				{
					tCNetmarbleDialog?.ReceivePrize(val2, val6, val3, val7, val8);
				}
			}
		}
	}

	private void HandleCS_MISSION_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out string val2);
		msg.Read(out int val3);
		msg.Read(out int val4);
		msg.Read(out bool val5);
		msg.Read(out int val6);
		MissionManager.Instance.UpdateAlways(val, val2, val3, val4, val5, val6);
	}

	private void HandleCS_MISSION_WAIT_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		MissionManager.Instance.SetMissionWait(val);
	}

	private void HandleCS_ACCEPT_DAILY_MISSION_ACK(MsgBody msg)
	{
		_waitingAck = false;
		msg.Read(out int val);
		if (val != 0)
		{
			switch (val)
			{
			case -1:
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("ACCEPT_ERR_ONCE_A_DAY"));
				break;
			case -2:
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("ACCEPT_ERR_ALREADY_ON_DUTY"));
				break;
			default:
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("ACCEPT_ERR_UNKNOWN"));
				break;
			}
		}
	}

	private void HandleCS_PROGRESS_DAILY_MISSION_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		MissionManager.Instance.Progress(val, val2);
	}

	private void HandleCS_COMPLETE_DAILY_MISSION_ACK(MsgBody msg)
	{
		_waitingAck = false;
		msg.Read(out int val);
		if (val > 0)
		{
			Dialog top = DialogManager.Instance.GetTop();
			if (top != null && top.ID == DialogManager.DIALOG_INDEX.MISSION)
			{
				MissionDialog missionDialog = (MissionDialog)top;
				missionDialog.Complete(val);
			}
			MissionManager.Instance.Complete(val);
		}
	}

	private void HandleCS_STARTING_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		if (val < 0)
		{
			GlobalVars.Instance.battleStarting = false;
		}
		else
		{
			if (!GlobalVars.Instance.battleStarting)
			{
				GlobalVars.Instance.BattleStarting();
				MyInfoManager.Instance.EnBattleStartRemain(StringMgr.Instance.Get("BATTLE_START"));
			}
			MyInfoManager.Instance.EnBattleStartRemain(val.ToString());
		}
	}

	private void HandleCS_RESET_USER_MAP_SLOTS_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		if (val == 0)
		{
			UserMapInfo userMapInfo = UserMapInfoManager.Instance.Get((byte)val2);
			if (userMapInfo != null && userMapInfo.Alias.Length > 0)
			{
				string msg2 = string.Format(StringMgr.Instance.Get("RESET_MAP_SLOT_SUCCESS"), userMapInfo.Alias);
				SystemMsgManager.Instance.ShowMessage(msg2);
			}
			UserMapInfoManager.Instance.Remove((byte)val2);
			UserMapInfoManager.Instance.ValidateEmpty();
		}
		else
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("FAIL_TO_RESET_MAP_SLOT"));
		}
	}

	private void HandleCS_INC_EXTRA_SLOTS_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		if (val != 0)
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("FAIL_TO_ADD_MAP_SLOT"));
		}
		else
		{
			MyInfoManager.Instance.ExtraSlots = val2;
			UserMapInfoManager.Instance.ValidateEmpty();
			SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("ADD_MAP_SLOT_SUCCESS"));
		}
	}

	private void HandleCS_CREATE_CHARACTER_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out string val2);
		msg.Read(out string val3);
		if (val == 0)
		{
			Application.LoadLevel("BfStart");
		}
		else
		{
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				PlayerInfoMain component = gameObject.GetComponent<PlayerInfoMain>();
				if (null != component)
				{
					component.IsCreating = false;
				}
			}
			string msg2 = string.Format(StringMgr.Instance.Get("FAIL_TO_CREATE_CHARACTER"), val2);
			switch (val)
			{
			case -2:
				msg2 = string.Format(StringMgr.Instance.Get("NICKNAME_DUPLICATE"), val2);
				break;
			case -32:
				msg2 = string.Format(StringMgr.Instance.Get("BAD_WORD_DETECT"), val3);
				break;
			}
			MessageBoxMgr.Instance.AddMessage(msg2);
		}
	}

	private void HandleCS_WEAPON_CHANGE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int _);
		msg.Read(out long val3);
		_waitingAck = false;
		if (val != 0)
		{
			switch (val)
			{
			case -4:
				SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("ERR_WPN_CHG_PREMIUM_ONLY"));
				break;
			case -3:
				SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("ERR_WPN_CHG_NO_MONEY"));
				break;
			case -2:
				SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("ERR_WPN_CHG_SAME_WEAPON"));
				break;
			}
		}
		else if (MyInfoManager.Instance.WeaponChange(val3))
		{
			GameObject gameObject = GameObject.Find("Me");
			if (null != gameObject)
			{
				gameObject.SendMessage("OnSwapWeapon");
				LocalController component = gameObject.GetComponent<LocalController>();
				if (null != component)
				{
					component.PickupFromInstance(val3);
				}
			}
		}
	}

	private void HandleCS_PLAYER_WEAPON_CHANGE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out string val2);
		msg.Read(out string val3);
		bool flag = false;
		BrickManDesc desc = BrickManManager.Instance.GetDesc(val);
		if (desc != null)
		{
			flag = desc.ChangeWeapon(val3);
		}
		if (flag)
		{
			GameObject gameObject = BrickManManager.Instance.Get(val);
			if (null != gameObject)
			{
				LookCoordinator component = gameObject.GetComponent<LookCoordinator>();
				if (null != component)
				{
					component.WeaponChange(val2, val3, val);
				}
			}
		}
	}

	private void HandleCS_GENERIC_BUNDLE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		for (int i = 0; i < val; i++)
		{
			msg.Read(out string val2);
			msg.Read(out string val3);
			msg.Read(out string val4);
			msg.Read(out string val5);
			TItemManager.Instance.UpdateBundle(val2, val3, val4, val5);
		}
	}

	private void HandleCS_CUSTOM_STRING_ACK(MsgBody msg)
	{
		msg.Read(out string val);
		msg.Read(out string val2);
		msg.Read(out string val3);
		msg.Read(out string val4);
		msg.Read(out string val5);
		msg.Read(out string val6);
		msg.Read(out string val7);
		msg.Read(out string val8);
		msg.Read(out string val9);
		msg.Read(out string val10);
		msg.Read(out string val11);
		msg.Read(out string val12);
		msg.Read(out string val13);
		StringMgr.Instance.UpdateStrings(val, val2, val3, val4, val5, val6, val7, val8, val9, val10, val11, val12, val13);
	}

	private void HandleCS_MAP_EVAL_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		RoomManager.Instance.commented = 1;
		if (val == 0)
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("MAP_EVAL_END"));
		}
		else
		{
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("MAP_EVAL_FAIL"));
		}
	}

	private void HandleCS_MY_DOWNLOAD_MAP_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		DownloadMapFrame downloadMapFrame = null;
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			Lobby component = gameObject.GetComponent<Lobby>();
			if (null != component)
			{
				downloadMapFrame = component.myMapFrm.downloadMapFrm;
			}
		}
		if (downloadMapFrame != null && val2 > 0)
		{
			downloadMapFrame.BeginMapList(val);
		}
		for (int i = 0; i < val2; i++)
		{
			msg.Read(out int val3);
			msg.Read(out string val4);
			msg.Read(out string val5);
			msg.Read(out ushort val6);
			msg.Read(out byte val7);
			msg.Read(out byte val8);
			msg.Read(out int val9);
			msg.Read(out sbyte val10);
			msg.Read(out sbyte val11);
			msg.Read(out sbyte val12);
			msg.Read(out sbyte val13);
			msg.Read(out sbyte val14);
			msg.Read(out int val15);
			msg.Read(out int val16);
			msg.Read(out int val17);
			msg.Read(out int val18);
			msg.Read(out int val19);
			msg.Read(out int val20);
			bool clanMatchable = (val7 & Room.clanMatch) != 0;
			bool officialMap = (val7 & Room.official) != 0;
			bool blocked = (val7 & Room.blocked) != 0;
			DateTime regDate = new DateTime(val9, val10, val11, val12, val13, val14);
			RegMap always = RegMapManager.Instance.GetAlways(val3, val4, val5, regDate, val6, clanMatchable, officialMap, val18, val19, val20, val15, val16, val17, val8, blocked);
			if (downloadMapFrame != null && always != null)
			{
				if (i == 0 && downloadMapFrame != null)
				{
					downloadMapFrame.firstIndexer = always.Map;
				}
				if (i == val2 - 1)
				{
					downloadMapFrame.lastIndexer = always.Map;
				}
			}
			if (always != null)
			{
				RegMapManager.Instance.SetDownload(val3, download: true);
			}
			else
			{
				Debug.LogError("Fail to get regmap: " + val3);
			}
		}
		downloadMapFrame?.EndMapList();
	}

	private void HandleCS_MY_REGISTER_MAP_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		MyRegMapFrame myRegMapFrame = null;
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			Lobby component = gameObject.GetComponent<Lobby>();
			if (null != component)
			{
				myRegMapFrame = component.myMapFrm.myRegMapFrm;
			}
		}
		if (myRegMapFrame != null && val2 > 0)
		{
			myRegMapFrame.BeginMapList(val);
		}
		for (int i = 0; i < val2; i++)
		{
			msg.Read(out int val3);
			msg.Read(out string val4);
			msg.Read(out string val5);
			msg.Read(out ushort val6);
			msg.Read(out byte val7);
			msg.Read(out byte val8);
			msg.Read(out int val9);
			msg.Read(out sbyte val10);
			msg.Read(out sbyte val11);
			msg.Read(out sbyte val12);
			msg.Read(out sbyte val13);
			msg.Read(out sbyte val14);
			msg.Read(out int val15);
			msg.Read(out int val16);
			msg.Read(out int val17);
			msg.Read(out int val18);
			msg.Read(out int val19);
			msg.Read(out int val20);
			bool clanMatchable = (val7 & Room.clanMatch) != 0;
			bool officialMap = (val7 & Room.official) != 0;
			bool blocked = (val7 & Room.blocked) != 0;
			DateTime regDate = new DateTime(val9, val10, val11, val12, val13, val14);
			RegMap always = RegMapManager.Instance.GetAlways(val3, val4, val5, regDate, val6, clanMatchable, officialMap, val18, val19, val20, val15, val16, val17, val8, blocked);
			if (myRegMapFrame != null && always != null)
			{
				if (i == 0)
				{
					myRegMapFrame.firstIndexer = always.Map;
				}
				if (i == val2 - 1)
				{
					myRegMapFrame.lastIndexer = always.Map;
				}
				myRegMapFrame.AddMapItem(always);
			}
		}
		myRegMapFrame?.EndMapList();
	}

	private void HandleCS_USER_MAP_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		EditingMapFrame editingMapFrame = null;
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			Lobby component = gameObject.GetComponent<Lobby>();
			if (null != component)
			{
				editingMapFrame = component.myMapFrm.editMapFrm;
			}
		}
		if (editingMapFrame != null && val2 + editingMapFrame.GetEmptyPremiumSlot(val, val2) > 0)
		{
			editingMapFrame.BeginMapList(val);
		}
		for (int i = 0; i < val2; i++)
		{
			msg.Read(out int val3);
			msg.Read(out string val4);
			msg.Read(out int val5);
			msg.Read(out int val6);
			msg.Read(out sbyte val7);
			msg.Read(out sbyte val8);
			msg.Read(out sbyte val9);
			msg.Read(out sbyte val10);
			msg.Read(out sbyte val11);
			msg.Read(out sbyte val12);
			DateTime lastModified = DateTime.MinValue;
			if (val4.Length > 0)
			{
				lastModified = ((val6 > 0) ? new DateTime(val6, val7, val8, val9, val10, val11) : new DateTime(1971, 12, 29));
			}
			UserMapInfoManager.Instance.AddOrUpdate(val3, val4, val5, lastModified, val12);
		}
	}

	private void HandleCS_ALL_MAP_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		SearchMapFrame searchMapFrame = null;
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			Lobby component = gameObject.GetComponent<Lobby>();
			if (null != component)
			{
				searchMapFrame = component.searchMapFrm;
			}
		}
		if (searchMapFrame != null && val2 > 0)
		{
			searchMapFrame.BeginMapList(val);
		}
		for (int i = 0; i < val2; i++)
		{
			msg.Read(out int val3);
			msg.Read(out string val4);
			msg.Read(out string val5);
			msg.Read(out ushort val6);
			msg.Read(out byte val7);
			msg.Read(out byte val8);
			msg.Read(out int val9);
			msg.Read(out sbyte val10);
			msg.Read(out sbyte val11);
			msg.Read(out sbyte val12);
			msg.Read(out sbyte val13);
			msg.Read(out sbyte val14);
			msg.Read(out int val15);
			msg.Read(out int val16);
			msg.Read(out int val17);
			msg.Read(out int val18);
			msg.Read(out int val19);
			msg.Read(out int val20);
			bool clanMatchable = (val7 & Room.clanMatch) != 0;
			bool officialMap = (val7 & Room.official) != 0;
			bool blocked = (val7 & Room.blocked) != 0;
			DateTime regDate = new DateTime(val9, val10, val11, val12, val13, val14);
			RegMap always = RegMapManager.Instance.GetAlways(val3, val4, val5, regDate, val6, clanMatchable, officialMap, val18, val19, val20, val15, val16, val17, val8, blocked);
			if (searchMapFrame != null && always != null)
			{
				if (i == 0)
				{
					searchMapFrame.firstIndexer = always.Map;
				}
				if (i == val2 - 1)
				{
					searchMapFrame.lastIndexer = always.Map;
				}
				searchMapFrame.AddMapItem(always);
			}
		}
		searchMapFrame?.EndMapList();
	}

	private void HandleCS_WEEKLY_CHART_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out int val3);
		msg.Read(out sbyte val4);
		msg.Read(out sbyte val5);
		msg.Read(out int val6);
		SearchMapFrame searchMapFrame = null;
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			Lobby component = gameObject.GetComponent<Lobby>();
			if (null != component)
			{
				searchMapFrame = component.searchMapFrm;
			}
		}
		if (searchMapFrame != null && val6 > 0)
		{
			searchMapFrame.BeginMapList(val);
			searchMapFrame.year4Week = val3;
			searchMapFrame.month4Week = val4;
			searchMapFrame.day4Week = val5;
			searchMapFrame.chartSeq = val2;
		}
		for (int i = 0; i < val6; i++)
		{
			msg.Read(out int val7);
			msg.Read(out int val8);
			msg.Read(out int val9);
			msg.Read(out string val10);
			msg.Read(out string val11);
			msg.Read(out ushort val12);
			msg.Read(out byte val13);
			msg.Read(out byte val14);
			msg.Read(out int val15);
			msg.Read(out sbyte val16);
			msg.Read(out sbyte val17);
			msg.Read(out sbyte val18);
			msg.Read(out sbyte val19);
			msg.Read(out sbyte val20);
			msg.Read(out int val21);
			msg.Read(out int val22);
			msg.Read(out int val23);
			msg.Read(out int val24);
			msg.Read(out int val25);
			msg.Read(out int val26);
			bool clanMatchable = (val13 & Room.clanMatch) != 0;
			bool officialMap = (val13 & Room.official) != 0;
			bool blocked = (val13 & Room.blocked) != 0;
			DateTime regDate = new DateTime(val15, val16, val17, val18, val19, val20);
			RegMap always = RegMapManager.Instance.GetAlways(val9, val10, val11, regDate, val12, clanMatchable, officialMap, val24, val25, val26, val21, val22, val23, val14, blocked);
			always.setRank(val7, val8);
			if (searchMapFrame != null && always != null)
			{
				if (i == 0)
				{
					searchMapFrame.firstIndexer = always.Map;
				}
				if (i == val6 - 1)
				{
					searchMapFrame.lastIndexer = always.Map;
				}
				searchMapFrame.AddMapItem(always);
			}
		}
		searchMapFrame?.EndMapList();
	}

	private void HandleCS_MAP_HONOR_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		SearchMapFrame searchMapFrame = null;
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			Lobby component = gameObject.GetComponent<Lobby>();
			if (null != component)
			{
				searchMapFrame = component.searchMapFrm;
			}
		}
		if (searchMapFrame != null && val2 > 0)
		{
			searchMapFrame.BeginMapList(val);
		}
		for (int i = 0; i < val2; i++)
		{
			msg.Read(out int val3);
			msg.Read(out int val4);
			msg.Read(out string val5);
			msg.Read(out string val6);
			msg.Read(out ushort val7);
			msg.Read(out byte val8);
			msg.Read(out byte val9);
			msg.Read(out int val10);
			msg.Read(out sbyte val11);
			msg.Read(out sbyte val12);
			msg.Read(out sbyte val13);
			msg.Read(out sbyte val14);
			msg.Read(out sbyte val15);
			msg.Read(out int val16);
			msg.Read(out int val17);
			msg.Read(out int val18);
			msg.Read(out int val19);
			msg.Read(out int val20);
			msg.Read(out int val21);
			bool clanMatchable = (val8 & Room.clanMatch) != 0;
			bool officialMap = (val8 & Room.official) != 0;
			bool blocked = (val8 & Room.blocked) != 0;
			DateTime regDate = new DateTime(val10, val11, val12, val13, val14, val15);
			RegMap always = RegMapManager.Instance.GetAlways(val4, val5, val6, regDate, val7, clanMatchable, officialMap, val19, val20, val21, val16, val17, val18, val9, blocked);
			if (searchMapFrame != null && always != null)
			{
				if (i == 0)
				{
					searchMapFrame.firstHonor = val3;
				}
				else if (i == val2 - 1)
				{
					searchMapFrame.lastHonor = val3;
				}
				searchMapFrame.AddMapItem(always);
			}
		}
		searchMapFrame?.EndMapList();
	}

	private void HandleCS_MAP_DETAIL_ACK(MsgBody msg)
	{
		msg.Read(out int _);
		msg.Read(out string val2);
		msg.Read(out int val3);
		msg.Read(out int val4);
		GlobalVars.Instance.totalComments = val3;
		GlobalVars.Instance.IsIntroChange = true;
		GlobalVars.Instance.IsPriceChange = true;
		GlobalVars.Instance.intro = val2;
		for (int i = 0; i < val4; i++)
		{
			msg.Read(out int val5);
			msg.Read(out string val6);
			msg.Read(out string val7);
			msg.Read(out byte val8);
			GlobalVars.Instance.AddComment(val5, val6, val7, val8);
		}
	}

	private void HandleCS_MORE_COMMENT_ACK(MsgBody msg)
	{
		msg.Read(out int _);
		msg.Read(out int val2);
		for (int i = 0; i < val2; i++)
		{
			msg.Read(out int val3);
			msg.Read(out string val4);
			msg.Read(out string val5);
			msg.Read(out byte val6);
			GlobalVars.Instance.AddComment(val3, val4, val5, val6);
		}
	}

	private void HandleCS_CHG_MAP_INTRO_ACK(MsgBody msg)
	{
		msg.Read(out int _);
		GlobalVars.Instance.IsIntroChangeTemp = true;
	}

	private void HandleCS_CHG_MAP_DOWNLOAD_FREE_ACK(MsgBody msg)
	{
		msg.Read(out int _);
		GlobalVars.Instance.IsPriceChangeTemp = true;
	}

	private void HandleCS_MAP_DAY_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		SearchMapFrame searchMapFrame = null;
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			Lobby component = gameObject.GetComponent<Lobby>();
			if (null != component)
			{
				searchMapFrame = component.searchMapFrm;
			}
		}
		if (searchMapFrame != null && val2 > 0)
		{
			searchMapFrame.BeginMapList(val);
		}
		for (int i = 0; i < val2; i++)
		{
			msg.Read(out int val3);
			msg.Read(out int val4);
			msg.Read(out string val5);
			msg.Read(out string val6);
			msg.Read(out ushort val7);
			msg.Read(out byte val8);
			msg.Read(out byte val9);
			msg.Read(out int val10);
			msg.Read(out sbyte val11);
			msg.Read(out sbyte val12);
			msg.Read(out sbyte val13);
			msg.Read(out sbyte val14);
			msg.Read(out sbyte val15);
			msg.Read(out int val16);
			msg.Read(out int val17);
			msg.Read(out int val18);
			msg.Read(out int val19);
			msg.Read(out int val20);
			msg.Read(out int val21);
			bool clanMatchable = (val8 & Room.clanMatch) != 0;
			bool officialMap = (val8 & Room.official) != 0;
			bool blocked = (val8 & Room.blocked) != 0;
			DateTime regDate = new DateTime(val10, val11, val12, val13, val14, val15);
			RegMap always = RegMapManager.Instance.GetAlways(val4, val5, val6, regDate, val7, clanMatchable, officialMap, val19, val20, val21, val16, val17, val18, val9, blocked);
			if (searchMapFrame != null && always != null)
			{
				if (i == 0)
				{
					searchMapFrame.firstHonor = val3;
				}
				else if (i == val2 - 1)
				{
					searchMapFrame.lastHonor = val3;
				}
				searchMapFrame.AddMapItem(always);
			}
		}
		searchMapFrame?.EndMapList();
	}

	private void HandleCS_CHG_DOOR_STATUS_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out sbyte val2);
		GameObject gameObject = GameObject.Find("Me");
		if (null != gameObject)
		{
			LocalController component = gameObject.GetComponent<LocalController>();
			if (null != component)
			{
				component.CommandOpenDoor(val, val2, soundOn: true);
			}
		}
	}

	private void HandleCS_OPEN_DOOR_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		GameObject gameObject = GameObject.Find("Me");
		if (null != gameObject)
		{
			LocalController component = gameObject.GetComponent<LocalController>();
			if (null != component)
			{
				component.CommandOpenDoor(val, 1, soundOn: false);
			}
		}
	}

	private void HandleCS_ROOM_CONFIG_FAIL_ACK(MsgBody msg)
	{
		msg.Read(out int _);
		MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("FAIL_TO_CHANGE_ROOM_CONFIG"));
	}

	private void HandleCS_WEAPON_MODIFIER_EX_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		for (int i = 0; i < val; i++)
		{
			msg.Read(out int val2);
			msg.Read(out float val3);
			msg.Read(out float val4);
			msg.Read(out int val5);
			msg.Read(out float val6);
			msg.Read(out int val7);
			msg.Read(out float val8);
			msg.Read(out float val9);
			msg.Read(out float val10);
			msg.Read(out int val11);
			msg.Read(out int val12);
			msg.Read(out int val13);
			msg.Read(out float val14);
			msg.Read(out float val15);
			WeaponModifier.Instance.UpdateWpnModEx(val2, val3, val4, val5, val6, val7, val8, val9, val10, val11, val12, val13, val14, val15);
		}
	}

	private void HandleCS_SVC_ENTER_LIST_ACK(MsgBody msg)
	{
		ChannelUserManager.Instance.Clear();
		msg.Read(out int val);
		for (int i = 0; i < val; i++)
		{
			msg.Read(out int val2);
			msg.Read(out string val3);
			msg.Read(out int val4);
			msg.Read(out int val5);
			ChannelUserManager.Instance.AddUser(val2, val3, XpManager.Instance.GetLevel(val4), val5);
		}
	}

	private void HandleCS_ROOM_LIST_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		RoomManager.Instance.ClearRooms();
		for (int i = 0; i < val; i++)
		{
			msg.Read(out int val2);
			msg.Read(out int val3);
			msg.Read(out string val4);
			msg.Read(out bool val5);
			msg.Read(out int val6);
			msg.Read(out int val7);
			msg.Read(out int val8);
			msg.Read(out int val9);
			msg.Read(out string val10);
			msg.Read(out int val11);
			msg.Read(out int val12);
			msg.Read(out int val13);
			msg.Read(out int val14);
			msg.Read(out int val15);
			msg.Read(out int val16);
			msg.Read(out int val17);
			msg.Read(out bool val18);
			msg.Read(out bool val19);
			msg.Read(out bool val20);
			msg.Read(out int val21);
			msg.Read(out int val22);
			RoomManager.Instance.AddOrUpdateRoom(val2, (Room.ROOM_STATUS)val6, val7, val8, val5, val9, val10, val11, val12, val13, val14, val15, val16, val17, val18, val3, val4, val19, val20, val21, val22);
		}
		RoomManager.Instance.RefreshRoomList();
	}

	private void HandleCS_ROOM_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out string val3);
		msg.Read(out bool val4);
		msg.Read(out int val5);
		msg.Read(out int val6);
		msg.Read(out int val7);
		msg.Read(out int val8);
		msg.Read(out string val9);
		msg.Read(out int val10);
		msg.Read(out int val11);
		msg.Read(out int val12);
		msg.Read(out int val13);
		msg.Read(out int val14);
		msg.Read(out int val15);
		msg.Read(out int val16);
		msg.Read(out bool val17);
		msg.Read(out bool val18);
		msg.Read(out bool val19);
		msg.Read(out int val20);
		msg.Read(out int val21);
		RoomManager.Instance.AddOrUpdateRoom(val, (Room.ROOM_STATUS)val5, val6, val7, val4, val8, val9, val10, val11, val12, val13, val14, val15, val16, val17, val2, val3, val18, val19, val20, val21);
		if (RoomManager.Instance.GetRoom(val) == null)
		{
			RoomManager.Instance.RefreshRoomList();
		}
	}

	private void HandleCS_XP_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		int[] array = new int[val];
		for (int i = 0; i < val; i++)
		{
			msg.Read(out int val2);
			array[i] = val2;
		}
		XpManager.Instance.SetXpTable(array);
	}

	public void SendCS_CHANNEL_PLAYER_LIST_REQ()
	{
		Say(478, new MsgBody());
	}

	public void SendCS_BUNGEE_SCORE_REQ()
	{
		Say(474, new MsgBody());
	}

	private void HandleCS_BUNGEE_SCORE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnBungeeScore", val);
		}
	}

	private void HandleCS_BUNGEE_MODE_END_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		List<ResultUnitEx> list = new List<ResultUnitEx>();
		for (int i = 0; i < val; i++)
		{
			msg.Read(out bool val2);
			msg.Read(out int val3);
			msg.Read(out string val4);
			msg.Read(out int val5);
			msg.Read(out int val6);
			msg.Read(out int val7);
			msg.Read(out int val8);
			msg.Read(out int val9);
			msg.Read(out int val10);
			msg.Read(out int val11);
			msg.Read(out int val12);
			msg.Read(out int val13);
			msg.Read(out long val14);
			msg.Read(out int val15);
			list.Add(new ResultUnitEx(val2, val3, val4, val5, val6, val7, val8, val9, val10, val11, val12, val13, val14, val15));
		}
		list.Sort((ResultUnitEx prev, ResultUnitEx next) => prev.Compare(next));
		RoomManager.Instance.RU = list.ToArray();
		RoomManager.Instance.endCode = 0;
		RoomManager.Instance.redTotalKill = 0;
		RoomManager.Instance.redTotalDeath = 0;
		RoomManager.Instance.blueTotalKill = 0;
		RoomManager.Instance.blueTotalDeath = 0;
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnMatchEnd", (object)(sbyte)0);
		}
	}

	private void HandleCS_CUR_CHANNEL_SPECIFIC_INFO_AC(MsgBody msg)
	{
		msg.Read(out int val);
		ChannelManager.Instance.Tk2FpMultiple = val;
	}

	public void SendCS_BATCH_DEL_BRICK_REQ(int[] bricks)
	{
		if (!_waitingAck)
		{
			MsgBody msgBody = new MsgBody();
			msgBody.Write(bricks.Length);
			for (int i = 0; i < bricks.Length; i++)
			{
				msgBody.Write(bricks[i]);
			}
			Say(479, msgBody);
			_waitingAck = true;
		}
	}

	private void HandleCS_BATCH_DEL_BRICK_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		bool flag = false;
		AudioSource audioSource = null;
		GameObject gameObject = GameObject.Find("Me");
		LocalController localController = null;
		if (null != gameObject)
		{
			localController = gameObject.GetComponent<LocalController>();
		}
		if (MyInfoManager.Instance.Seq == val)
		{
			_waitingAck = false;
		}
		for (int i = 0; i < val2; i++)
		{
			msg.Read(out int val3);
			if (MyInfoManager.Instance.Seq == val)
			{
				if (null != localController)
				{
					audioSource = localController.GetComponent<AudioSource>();
				}
				if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BUNGEE)
				{
					GameObject[] array = BrickManManager.Instance.ToGameObjectArray();
					for (int j = 0; j < array.Length; j++)
					{
						TPController component = array[j].GetComponent<TPController>();
						GameObject brickObject = BrickManager.Instance.GetBrickObject(val3);
						if (brickObject != null)
						{
							float num = Vector3.Distance(brickObject.transform.position, component.transform.position);
							if (num < 3f)
							{
								UnityEngine.Object.Instantiate((UnityEngine.Object)BrickManager.Instance.bungeeFeedbackEffects, brickObject.transform.position, brickObject.transform.rotation);
								GameObject gameObject2 = GameObject.Find("Main");
								if (null != gameObject2)
								{
									BungeeMatch component2 = gameObject2.GetComponent<BungeeMatch>();
									if (component2 != null)
									{
										component2.OnEffectivePoint(brickObject.transform.position, num);
									}
								}
								break;
							}
						}
					}
				}
			}
			else
			{
				GameObject gameObject3 = BrickManManager.Instance.Get(val);
				if (gameObject3 != null)
				{
					audioSource = gameObject3.GetComponent<AudioSource>();
				}
				if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.BUNGEE)
				{
					GameObject brickObject2 = BrickManager.Instance.GetBrickObject(val3);
					if (brickObject2 != null)
					{
						float num2 = Vector3.Distance(brickObject2.transform.position, localController.TranceformPosition);
						int num3 = 0;
						if (num2 < 1f)
						{
							num3 = 3;
						}
						else if (num2 < 2f)
						{
							num3 = 2;
						}
						else if (num2 < 3f)
						{
							num3 = 1;
						}
						if (num3 > 0)
						{
							Vector3 position = brickObject2.transform.position;
							float y = position.y;
							Vector3 tranceformPosition = localController.TranceformPosition;
							if (y <= tranceformPosition.y)
							{
								num3++;
							}
						}
						localController.LogAttacker(val, num3);
					}
				}
			}
			if (BuildOption.Instance.Props.brickSoundChange && !flag)
			{
				BrickInst brickInst = BrickManager.Instance.userMap.Get(val3);
				if (brickInst != null)
				{
					Brick brick = BrickManager.Instance.GetBrick(brickInst.Template);
					if (brick != null && audioSource != null && brick.delSound != null)
					{
						flag = true;
						audioSource.PlayOneShot(brick.delSound);
					}
				}
			}
			BrickManager.Instance.DelBrick(val3, shrink: true);
		}
		MyInfoManager.Instance.IsModified = true;
	}

	public void SendCS_INVITE_REQ(int inviteeSeq, string nickName)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(inviteeSeq);
		msgBody.Write(nickName);
		Say(481, msgBody);
	}

	private void HandleCS_INVITE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int _);
		msg.Read(out string val3);
		string text = "HandleCS_INVITE_ACK UnKnown ret";
		switch (val)
		{
		case 0:
			text = string.Format(StringMgr.Instance.Get("INVITE_SUCCESS"), val3);
			break;
		case -1:
			text = StringMgr.Instance.Get("INVITE_FAIL_NO_ROOM");
			break;
		case -2:
			text = string.Format(StringMgr.Instance.Get("INVITE_FAIL_INVITEE_BUSY"), val3);
			break;
		case -3:
			text = string.Format(StringMgr.Instance.Get("USER_NOT_FOUND"), val3);
			break;
		case -4:
			text = StringMgr.Instance.Get("EXCEED");
			break;
		case -6:
			text = StringMgr.Instance.Get("LOADING_WAIT");
			break;
		case -7:
			text = StringMgr.Instance.Get("CANT_BREAK_INTO");
			break;
		case -8:
			text = StringMgr.Instance.Get("INVITE_FAIL_CLAN_MATCH");
			break;
		case -9:
			text = string.Format(StringMgr.Instance.Get("INVITE_FAIL_DENY_ALL"), val3);
			break;
		case -10:
			text = string.Format(StringMgr.Instance.Get("INVITE_FAIL_BAN"), val3);
			break;
		case -11:
			text = string.Format(StringMgr.Instance.Get("INVITE_FAIL_DENY_ALL"), val3);
			break;
		case -12:
			text = StringMgr.Instance.Get("INVITE_FAIL_KICKED");
			break;
		}
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, string.Empty, text));
		}
	}

	private void HandleCS_INVITED_ACK(MsgBody msg)
	{
		Invite invite = new Invite();
		msg.Read(out invite.invitorSeq);
		msg.Read(out invite.invitorNickname);
		msg.Read(out invite.channelIndex);
		msg.Read(out invite.roomNo);
		msg.Read(out invite.mode);
		msg.Read(out invite.pswd);
		msg.Read(out invite.clanSeq);
		msg.Read(out invite.squadIndex);
		msg.Read(out invite.squadCounterIndex);
		if (invite.squadIndex < 0)
		{
			GlobalVars.Instance.clanSendJoinREQ = -1;
		}
		else
		{
			GlobalVars.Instance.clanSendJoinREQ = 1;
		}
		InviteManager.Instance.AddInvite(invite);
	}

	public void SendCS_FOLLOWING_REQ(int followeeSeq, string nickName)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(followeeSeq);
		msgBody.Write(nickName);
		Say(484, msgBody);
	}

	private void HandleCS_FOLLOWING_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int _);
		msg.Read(out string val3);
		msg.Read(out int val4);
		msg.Read(out int val5);
		switch (val)
		{
		case -11:
		case -9:
			break;
		case 0:
			Compass.Instance.SetDestination(Compass.DESTINATION_LEVEL.ROOM, val4, val5, string.Empty);
			break;
		case -1:
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("FOLLOW_FAIL_ALREADY_ROOM"));
			break;
		case -2:
			MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("FOLLOW_FAIL_FOLLOWEE_NO_ROOM"), val3));
			break;
		case -3:
			MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("USER_NOT_FOUND"), val3));
			break;
		case -4:
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("EXCEED"));
			break;
		case -5:
			MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("FOLLOW_FAIL_LOCKED"), val3));
			break;
		case -6:
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("LOADING_WAIT"));
			break;
		case -7:
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("CANT_BREAK_INTO"));
			break;
		case -8:
			MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("FOLLOW_FAIL_CLAN_MATCH"), val3));
			break;
		case -10:
			MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("FOLLOW_FAIL_BAN"), val3));
			break;
		case -12:
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("FOLLOW_FAIL_KICKED"));
			break;
		case -13:
			MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("FOLLOW_FAIL_BAN"), val3));
			break;
		}
	}

	private void HandleCS_NMPLAYAUTH_STATE_NOTI_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		switch (val2)
		{
		case 60:
			switch (val)
			{
			case 1:
			{
				string arg4 = (val2 / 60).ToString() + StringMgr.Instance.Get("HOURS");
				string msg5 = string.Format(StringMgr.Instance.Get("SHUTDOWN_WARNING_01"), arg4);
				SystemMsgManager.Instance.ShowMessage(msg5);
				break;
			}
			case 2:
			{
				string arg3 = (val2 / 60).ToString() + StringMgr.Instance.Get("HOURS");
				string msg4 = string.Format(StringMgr.Instance.Get("PARENTS_ALLOW_OVER_TIME"), arg3);
				SystemMsgManager.Instance.ShowMessage(msg4);
				break;
			}
			}
			break;
		case 30:
			switch (val)
			{
			case 1:
			{
				string arg2 = val2.ToString() + StringMgr.Instance.Get("MINUTES");
				string msg3 = string.Format(StringMgr.Instance.Get("SHUTDOWN_WARNING_01"), arg2);
				SystemMsgManager.Instance.ShowMessage(msg3);
				break;
			}
			case 2:
			{
				string arg = val2.ToString() + StringMgr.Instance.Get("MINUTES");
				string msg2 = string.Format(StringMgr.Instance.Get("PARENTS_ALLOW_OVER_TIME"), arg);
				SystemMsgManager.Instance.ShowMessage(msg2);
				break;
			}
			}
			break;
		case 0:
			GlobalVars.Instance.shutdownNow = true;
			CSNetManager.Instance.Sock.SendCS_LOGOUT_REQ();
			CSNetManager.Instance.Sock.Close();
			P2PManager.Instance.Shutdown();
			switch (val)
			{
			case 1:
				MessageBoxMgr.Instance.AddQuitMesssge(StringMgr.Instance.Get("SHUTDOWN_WARNING_02"));
				break;
			case 2:
				MessageBoxMgr.Instance.AddQuitMesssge(StringMgr.Instance.Get("PARENTS_ALLOW_OVER"));
				break;
			}
			break;
		}
	}

	private void HandleCS_CPTR_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out int val3);
		msg.Read(out int val4);
		msg.Read(out int val5);
		MyInfoManager.Instance.UseLongTimePlayReward = true;
		MyInfoManager.Instance.IsLongTimePlayActive = (val2 != val5);
		((LongTimePlayDialog)DialogManager.Instance.GetDialogAlways(DialogManager.DIALOG_INDEX.LONG_TIME_PLAY))?.SetData(val, val2, val3, val4, val5);
	}

	private void HandleCS_CPTR_REWARD_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		string text = string.Format(arg0: (!BuffManager.Instance.IsPCBangBuff()) ? StringMgr.Instance.Get("PLAY_REWARD_NORMAL01") : StringMgr.Instance.Get("PLAY_REWARD_PCBANG01"), format: StringMgr.Instance.Get("PLAY_REWARD_NORMAL07"), arg1: val.ToString());
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, 0, string.Empty, text));
		}
		SystemMsgManager.Instance.ShowMessage(text);
	}

	private void HandleCS_LEVELUP_REWARD_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		LevelUpCompensationManager.Instance.Clear();
		for (int i = 0; i < val; i++)
		{
			msg.Read(out int val2);
			msg.Read(out string val3);
			msg.Read(out int val4);
			msg.Read(out byte val5);
			LevelUpCompensationManager.Instance.Add(val2, val3, val4, val5);
		}
	}

	private void HandleCS_MISSION_REWARD_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		for (int i = 0; i < val; i++)
		{
			msg.Read(out sbyte val2);
			msg.Read(out int val3);
			msg.Read(out int val4);
			MissionLoadManager.Instance.ChangeReward(val2, val3, val4);
		}
	}

	private void HandleCS_ITEM_PROPERTY_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		for (int i = 0; i < val; i++)
		{
			msg.Read(out string val2);
			msg.Read(out string val3);
			msg.Read(out int val4);
			msg.Read(out string val5);
			msg.Read(out float val6);
			TCharacter tCharacter = TItemManager.Instance.Get<TCharacter>(val2);
			if (tCharacter != null && val3.Length > 1)
			{
				tCharacter.tBuff = BuffManager.Instance.Get(val3);
			}
			TCostume tCostume = TItemManager.Instance.Get<TCostume>(val2);
			if (tCostume != null)
			{
				tCostume.resetArmor(val4);
				if (val3.Length > 1)
				{
					tCostume.tBuff = BuffManager.Instance.Get(val3);
				}
			}
			TAccessory tAccessory = TItemManager.Instance.Get<TAccessory>(val2);
			if (tAccessory != null)
			{
				tAccessory.resetArmor(val4);
				if (val3.Length > 1)
				{
					tAccessory.tBuff = BuffManager.Instance.Get(val3);
				}
				if (val5.Length > 1)
				{
					tAccessory.functionMask = TItem.String2FunctionMask(val5);
				}
				tAccessory.functionFactor = val6;
			}
		}
	}

	private void HandleCS_PREMIUM_ITEM_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		string[] array = new string[val];
		for (int i = 0; i < val; i++)
		{
			msg.Read(out string val2);
			array[i] = val2;
		}
		PremiumItemManager.Instance.SetPremiumItems(array);
	}

	private void HandleCS_NETCAFE_ITEM_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		string[] array = new string[val];
		for (int i = 0; i < val; i++)
		{
			msg.Read(out string val2);
			array[i] = val2;
		}
		PremiumItemManager.Instance.SetPCBangItems(array);
	}

	public void SendCS_START_KICKOUT_VOTE_REQ(int target, int reason)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(target);
		msgBody.Write(reason);
		Say(494, msgBody);
	}

	public void SendCS_USE_CHANGE_NICKNAME_REQ(string newNickname, long target, string ticketCode)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(newNickname);
		msgBody.Write(target);
		msgBody.Write(ticketCode);
		Say(501, msgBody);
	}

	private void HandleCS_START_KICKOUT_VOTE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		switch (val)
		{
		case -1:
			SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("KICK_VOTE_MESSAGE10"));
			break;
		case -2:
			SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("KICK_VOTE_MESSAGE12"));
			break;
		case -3:
			SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("KICK_VOTE_MESSAGE11"));
			break;
		case -4:
			SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("KICK_VOTE_MESSAGE13"));
			break;
		case -5:
		{
			MenuEx menuEx = (MenuEx)DialogManager.Instance.GetDialogAlways(DialogManager.DIALOG_INDEX.MENU_EX);
			if (menuEx != null)
			{
				SystemMsgManager.Instance.ShowMessage(string.Format(StringMgr.Instance.Get("KICK_VOTE_MESSAGE01"), menuEx.kickOutVoteQuorum));
			}
			break;
		}
		case -6:
			SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("KICK_VOTE_MESSAGE14"));
			break;
		case -64:
			SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("KICK_VOTE_MESSAGE15"));
			break;
		}
	}

	public void SendCS_KICKOUT_VOTE_REQ(bool yes)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(yes);
		Say(496, msgBody);
	}

	private void HandleCS_KICKOUT_VOTE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		switch (val)
		{
		case -2:
			SystemMsgManager.Instance.ShowMessage(string.Format(StringMgr.Instance.Get("KICK_VOTE_MESSAGE07"), MyInfoManager.Instance.Nickname));
			break;
		case -3:
			SystemMsgManager.Instance.ShowMessage(string.Format(StringMgr.Instance.Get("KICK_VOTE_MESSAGE07"), MyInfoManager.Instance.Nickname));
			break;
		case -4:
			SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("KICK_VOTE_MESSAGE16"));
			break;
		case -64:
			SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("KICK_VOTE_MESSAGE15"));
			break;
		}
	}

	private void HandleCS_KICKOUT_VOTE_STATUS_ACK(MsgBody msg)
	{
		VoteStatus voteStatus = new VoteStatus();
		msg.Read(out voteStatus.yes);
		msg.Read(out voteStatus.no);
		msg.Read(out voteStatus.total);
		msg.Read(out voteStatus.reason);
		msg.Read(out voteStatus.target);
		msg.Read(out voteStatus.targetNickname);
		msg.Read(out voteStatus.isVoteAble);
		msg.Read(out voteStatus.isVoted);
		msg.Read(out voteStatus.remainTime);
		voteStatus.SetMakeTime();
		RoomManager.Instance.StartVoteStatus(voteStatus);
	}

	private void HandleCS_KICKOUT_VOTE_END_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out string val3);
		if (val == 1)
		{
			SystemMsgManager.Instance.ShowMessage(string.Format(StringMgr.Instance.Get("KICK_VOTE_MESSAGE08"), val3));
			if (val2 == MyInfoManager.Instance.Seq)
			{
				P2PManager.Instance.Shutdown();
				DialogManager.Instance.CloseAll();
				ContextMenuManager.Instance.CloseAll();
				MessageBoxMgr.Instance.AddMessage(string.Format(StringMgr.Instance.Get("HAS_BEEN_KICKED"), MyInfoManager.Instance.Nickname));
				SendCS_MAKE_ROOM_NULL_REQ();
				Application.LoadLevel("Lobby");
			}
		}
		else
		{
			SystemMsgManager.Instance.ShowMessage(string.Format(StringMgr.Instance.Get("KICK_VOTE_MESSAGE09"), val3));
		}
		RoomManager.Instance.ClearVote();
	}

	private void HandleCS_CUSTOM_GAME_CONFIG_ACK(MsgBody msg)
	{
		msg.Read(out bool val);
		msg.Read(out int val2);
		msg.Read(out bool val3);
		msg.Read(out int val4);
		msg.Read(out int val5);
		msg.Read(out int val6);
		MenuEx menuEx = (MenuEx)DialogManager.Instance.GetDialogAlways(DialogManager.DIALOG_INDEX.MENU_EX);
		if (menuEx != null)
		{
			menuEx.useKickOutVote = val;
			menuEx.kickOutVoteQuorum = val2;
		}
		CustomGameConfig.useRandomInvite = val3;
		CustomGameConfig.limitChatTime = val4;
		CustomGameConfig.limitChatCount = val5;
		CustomGameConfig.chatBlockTime = val6;
	}

	private void HandleCS_USE_CHANGE_NICKNAME_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out string val2);
		msg.Read(out string _);
		if (val == 0)
		{
			MyInfoManager.Instance.Nickname = val2;
			DialogManager.Instance.CloseAll();
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("SYS_NICKNAME_CHANGE_02"));
		}
		else
		{
			string text = string.Empty;
			switch (val)
			{
			case -1:
				text = StringMgr.Instance.Get("DUPLICATE_NAME");
				break;
			case -2:
				text = StringMgr.Instance.Get("SYSTEM_ERROR");
				break;
			case -3:
				text = StringMgr.Instance.Get("SYS_NICKNAME_CHANGE_03");
				break;
			case -4:
				text = StringMgr.Instance.Get("SYS_NICKNAME_CHANGE_04");
				break;
			case -5:
				text = StringMgr.Instance.Get("BANNED_STR");
				break;
			case -64:
				text = StringMgr.Instance.Get("SYSTEM_ERROR");
				break;
			}
			if (text.Length > 0)
			{
				MessageBoxMgr.Instance.AddMessage(text);
			}
		}
	}

	private void HandleCS_NICKNAME_CHANGE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out string val2);
		NameCard ban = MyInfoManager.Instance.GetBan(val);
		if (ban != null)
		{
			ban.Nickname = val2;
		}
		NameCard friend = MyInfoManager.Instance.GetFriend(val);
		if (friend != null)
		{
			friend.Nickname = val2;
		}
		NameCard clanee = MyInfoManager.Instance.GetClanee(val);
		if (clanee != null)
		{
			clanee.Nickname = val2;
		}
		BrickManDesc desc = BrickManManager.Instance.GetDesc(val);
		if (desc != null)
		{
			desc.Nickname = val2;
		}
	}

	public void SendCS_LOGIN_TO_AXESO5_REQ(string id, string pswd, int major, int minor)
	{
		if (!_waitingAck)
		{
			MsgBody msgBody = new MsgBody();
			msgBody.Write(id);
			msgBody.Write(pswd);
			msgBody.Write(major);
			msgBody.Write(minor);
			msgBody.Write(privateIpAddress);
			msgBody.Write(macAddress);
			Say(504, msgBody);
			_waitingAck = true;
		}
	}

	public void SendCS_MISSION_POINT_REQ(int redPoint, int bluePoint)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(redPoint);
		msgBody.Write(bluePoint);
		Say(508, msgBody);
	}

	private void HandleCS_BND_STATUS_ACK(MsgBody msg)
	{
        Debug.LogWarning("BND Status ACK");
		msg.Read(out bool val);
		MyInfoManager.Instance.BndModeDesc = new BuildNDestroyModeDesc();
		MyInfoManager.Instance.BndModeDesc.buildPhase = val;
	}

	private void HandleCS_LOGIN_TO_AXESO5_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out bool val3);
		_waitingAck = false;
		if (val >= 0)
		{
			if (GlobalVars.Instance.bRemember)
			{
				PlayerPrefs.SetString("myID", GlobalVars.Instance.strMyID);
			}
			else
			{
				PlayerPrefs.SetString("myID", string.Empty);
			}
			MyInfoManager.Instance.Seq = val;
			ChannelManager.Instance.LoginChannelId = val2;
			MyInfoManager.Instance.NeedPlayerInfo = val3;
			if (!val3)
			{
				if (BuildOption.Instance.Props.ShowAgb && !MyInfoManager.Instance.AgreeTos)
				{
					Application.LoadLevel("Tos");
				}
				else
				{
					Application.LoadLevel("BfStart");
				}
			}
			else
			{
				MyInfoManager.Instance.AgreeTos = false;
				if (BuildOption.Instance.Props.ShowAgb && !MyInfoManager.Instance.AgreeTos)
				{
					Application.LoadLevel("Tos");
				}
				else
				{
					Application.LoadLevel("PlayerInfo");
				}
			}
		}
		else
		{
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				gameObject.BroadcastMessage("OnLoginFailMessage", GetLoginFailString(val));
			}
		}
	}

	private void HandleCS_BUNGEE_STATUS_ACK(MsgBody msg)
	{
		P2PManager.Instance.SendPEER_BUNGEE_BREAK_INTO_REQ(MyInfoManager.Instance.Seq);
	}

	private void HandleCS_MISSION_POINT_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		if (MyInfoManager.Instance.Seq != RoomManager.Instance.Master)
		{
			DefenseManager.Instance.RedPoint = val;
			DefenseManager.Instance.BluePoint = val2;
		}
	}

	public void SendCS_ACCUSE_PLAYER_REQ(int reason, string nickName, string contents)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(reason);
		msgBody.Write(nickName);
		msgBody.Write(contents);
		Say(510, msgBody);
	}

	private void HandleCS_ACCUSE_PLAYER_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		if (val != 0 && val != -3)
		{
			DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.ACCUSATION, exclusive: true);
		}
		switch (val)
		{
		case 0:
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("REPORT_GM_MESSAGE_07"));
			break;
		case -1:
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("REPORT_GM_MESSAGE_03"));
			break;
		case -2:
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("REPORT_GM_MESSAGE_01"));
			break;
		case -3:
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("REPORT_GM_MESSAGE_02"));
			break;
		case -4:
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("REPORT_GM_MESSAGE_05"));
			break;
		case -5:
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("REPORT_GM_MESSAGE_06"));
			break;
		}
	}

	public void SendCS_ACCUSE_MAP_REQ(int reason, int seq, string contents)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(reason);
		msgBody.Write(seq);
		msgBody.Write(contents);
		Say(512, msgBody);
	}

	private void HandleCS_ACCUSE_MAP_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		if (val == -5)
		{
			DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.ACCUSATION_MAP, exclusive: true);
		}
		switch (val)
		{
		case -4:
			break;
		case 0:
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("REPORT_GM_MESSAGE_07"));
			break;
		case -1:
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("REPORT_GM_MESSAGE_03"));
			break;
		case -2:
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("REPORT_GM_MESSAGE_01"));
			break;
		case -3:
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("REPORT_GM_MESSAGE_02"));
			break;
		case -5:
			MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("REPORT_GM_MESSAGE_06"));
			break;
		}
	}

	public void SendCS_RESET_BATTLE_RECORD_REQ(int target, long ticket, string ticketCode)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(target);
		msgBody.Write(ticket);
		msgBody.Write(ticketCode);
		Say(514, msgBody);
	}

	private void HandleCS_RESET_BATTLE_RECORD_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		if (val >= 0)
		{
			string arg = string.Empty;
			switch (val)
			{
			case 0:
				arg = StringMgr.Instance.Get("RECORD_TYPE_FULLY");
				break;
			case 1:
				arg = StringMgr.Instance.Get("ROOM_TYPE_TEAM_MATCH");
				break;
			case 2:
				arg = StringMgr.Instance.Get("ROOM_TYPE_INDIVIDUAL_MATCH");
				break;
			case 7:
				arg = StringMgr.Instance.Get("ROOM_TYPE_BUNGEE");
				break;
			case 4:
				arg = StringMgr.Instance.Get("ROOM_TYPE_EXPLOSION");
				break;
			case 5:
				arg = StringMgr.Instance.Get("ROOM_TYPE_MISSION");
				break;
			case 6:
				arg = StringMgr.Instance.Get("ROOM_TYPE_BND");
				break;
			case 3:
				arg = StringMgr.Instance.Get("ROOM_TYPE_CAPTURE_THE_FLAG");
				break;
			case 16:
				arg = StringMgr.Instance.Get("RECORD_TYPE_WEAPON");
				break;
			}
			SystemMsgManager.Instance.ShowMessage(string.Format(StringMgr.Instance.Get("RECORD_SYS_MASSEGE"), arg));
		}
		else
		{
			switch (val)
			{
			case -1:
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("RECORD_ITEM_USE_ERROR01"));
				break;
			case -2:
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("RECORD_ITEM_USE_ERROR02"));
				break;
			case -3:
				MessageBoxMgr.Instance.AddMessage(StringMgr.Instance.Get("RECORD_ITEM_USE_ERROR03"));
				break;
			}
		}
	}

	public void SendCS_GM_COMMAND_USAGE_LOG_REQ(int gmCommand)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(gmCommand);
		Say(516, msgBody);
	}

	public void SendCS_RANDOM_INVITE_REQ()
	{
		MsgBody msgBody = new MsgBody();
		Say(517, msgBody);
	}

	public void SendCS_DROP_ITEM_REQ(string itemCode, int count, int subCount, Vector3 pos)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(itemCode);
		msgBody.Write(count);
		msgBody.Write(subCount);
		msgBody.Write(pos.x);
		msgBody.Write(pos.y);
		msgBody.Write(pos.z);
		Say(526, msgBody);
	}

	public void SendCS_MASTER_KICKING_REQ(int countdown)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(countdown);
		Say(534, msgBody);
	}

	public void SendCS_PICKUP_DROPPED_ITEM_REQ(int itemSeq)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(itemSeq);
		Say(528, msgBody);
	}

	private void HandleCS_RANDOM_INVITE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int _);
		msg.Read(out string val3);
		string text = string.Empty;
		switch (val)
		{
		case 0:
			text = string.Format(StringMgr.Instance.Get("RANDOM_INVITE_SUCCESS_MASSEGE"), val3);
			break;
		case -1:
			text = StringMgr.Instance.Get("RANDOM_INVITE_NO_PLAYER_MASSEGE");
			break;
		case -2:
			text = StringMgr.Instance.Get("RANDOM_INVITE_MASTER_ONLY_MASSEGE");
			break;
		case -3:
			text = StringMgr.Instance.Get("RANDOM_INVITE_NOT_SUPPORT_MASSEGE");
			break;
		}
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, string.Empty, text));
		}
	}

	private void HandleCS_DROP_ITEM_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out string val2);
		msg.Read(out int val3);
		msg.Read(out int val4);
		msg.Read(out float val5);
		msg.Read(out float val6);
		msg.Read(out float val7);
		GlobalVars.Instance.DropWeapon3P(val, val2, val3, val4, val5, val6, val7);
	}

	public void SendCS_ESCAPE_SCORE_REQ()
	{
		MsgBody msgBody = new MsgBody();
		Say(519, msgBody);
	}

	private void HandleCS_ESCAPE_SCORE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnEscapeScore", val);
		}
	}

	private void HandleCS_ESCAPE_MODE_END_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		List<ResultUnitEscape> list = new List<ResultUnitEscape>();
		for (int i = 0; i < val; i++)
		{
			msg.Read(out bool val2);
			msg.Read(out int val3);
			msg.Read(out string val4);
			msg.Read(out int val5);
			msg.Read(out int val6);
			msg.Read(out int val7);
			msg.Read(out int val8);
			msg.Read(out int val9);
			msg.Read(out int val10);
			msg.Read(out int val11);
			msg.Read(out int val12);
			msg.Read(out int val13);
			msg.Read(out long val14);
			msg.Read(out int val15);
			list.Add(new ResultUnitEscape(val2, val3, val4, val5, val6, val7, val8, val9, val10, val11, val12, val13, val14, val15));
		}
		list.Sort((ResultUnitEscape prev, ResultUnitEscape next) => prev.Compare(next));
		RoomManager.Instance.RU = list.ToArray();
		RoomManager.Instance.endCode = 0;
		RoomManager.Instance.redTotalKill = 0;
		RoomManager.Instance.redTotalDeath = 0;
		RoomManager.Instance.blueTotalKill = 0;
		RoomManager.Instance.blueTotalDeath = 0;
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnMatchEnd", (object)(sbyte)0);
		}
	}

	private void HandleCS_ESCAPE_STATUS_ACK(MsgBody msg)
	{
	}

	public void SendCS_ESCAPE_GOAL_REQ()
	{
		MsgBody msgBody = new MsgBody();
		Say(523, msgBody);
	}

	private void HandleCS_ESCAPE_GOAL_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out int val3);
		GameObject gameObject = GameObject.Find("Main");
		if (val == MyInfoManager.Instance.Seq)
		{
			Escape escape = null;
			if (null != gameObject)
			{
				escape = gameObject.GetComponent<Escape>();
				if (null != escape)
				{
					escape.GoalRespawn();
				}
				SerialKill component = gameObject.GetComponent<SerialKill>();
				if (null != component)
				{
					component.GoalInAction();
				}
			}
			MyInfoManager.Instance.Kill = val2;
			MyInfoManager.Instance.Score = val3;
		}
		else
		{
			BrickManDesc desc = BrickManManager.Instance.GetDesc(val);
			if (desc != null)
			{
				desc.Kill = val2;
				desc.Score = val3;
			}
		}
		bool flag = false;
		if (null != gameObject)
		{
			EscapeRanking component2 = gameObject.GetComponent<EscapeRanking>();
			if (null != component2)
			{
				component2.UpdateRanking();
				flag = component2.IsUpRankingBySeq(val);
			}
		}
		string empty = string.Empty;
		string victimNickname = string.Empty;
		if (val == MyInfoManager.Instance.Seq)
		{
			victimNickname = MyInfoManager.Instance.Nickname;
		}
		else
		{
			BrickManDesc desc2 = BrickManManager.Instance.GetDesc(val);
			if (desc2 != null)
			{
				victimNickname = desc2.Nickname;
			}
		}
		GameObject gameObject2 = GameObject.Find("Main");
		if (null != gameObject2)
		{
			Texture2D headshotImage = null;
			if (flag)
			{
				headshotImage = GlobalVars.Instance.GetWeaponByExcetion(-9);
			}
			Texture2D weaponByExcetion = GlobalVars.Instance.GetWeaponByExcetion(-8);
			gameObject2.BroadcastMessage("OnKillLog", new KillInfo(empty, victimNickname, weaponByExcetion, headshotImage, -1, val, -8));
		}
	}

	public void SendCS_ESCAPE_ACTIVE_PLAYER_REQ(bool IsActive)
	{
		MsgBody msgBody = new MsgBody();
		msgBody.Write(IsActive);
		Say(525, msgBody);
	}

	private void HandleCS_PICKUP_DROPPED_ITEM_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		if (MyInfoManager.Instance.Seq == val)
		{
			GlobalVars.Instance.SwapWeapon1P(val2);
		}
		else
		{
			GlobalVars.Instance.SwapWeapon3P(val, val2);
		}
	}

	private void HandleCS_DEL_DROP_ITEM_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		GlobalVars.Instance.DropWeaponDestroy(val);
	}

	private void HandleCS_DROPITEM_QUICK_JOIN_USER_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		for (int i = 0; i < val; i++)
		{
			msg.Read(out int val2);
			msg.Read(out string val3);
			msg.Read(out int val4);
			msg.Read(out int val5);
			msg.Read(out float val6);
			msg.Read(out float val7);
			msg.Read(out float val8);
			GlobalVars.Instance.DropWeapon3P(val2, val3, val4, val5, val6, val7, val8);
		}
	}

	private void HandleCS_STAR_RATE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		for (int i = 0; i < val; i++)
		{
			msg.Read(out string val2);
			msg.Read(out int val3);
			TItem tItem = TItemManager.Instance.Get<TItem>(val2);
			if (tItem != null)
			{
				tItem._StarRate = val3;
			}
		}
	}

	private void HandleCS_PICKUP_DROPPED_ITEM_FAIL_ACK(MsgBody msg)
	{
		msg.Read(out int _);
	}

	private void HandleCS_MASTER_KICKING_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			string text = string.Format(StringMgr.Instance.Get("ROOM_MASTER_KICKED_WARNING"), val);
			gameObject.BroadcastMessage("OnChat", new ChatText(ChatText.CHAT_TYPE.SYSTEM, -1, string.Empty, text));
		}
	}

	private void HandleCS_ZOMBIE_MODE_SCORE_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnZombieScore", new RoundScore(val2, val));
		}
	}

	private void HandleCS_ZOMBIE_END_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		List<ResultUnitZombie> list = new List<ResultUnitZombie>();
		for (int i = 0; i < val; i++)
		{
			msg.Read(out bool val2);
			msg.Read(out int val3);
			msg.Read(out string val4);
			msg.Read(out int val5);
			msg.Read(out int val6);
			msg.Read(out int val7);
			msg.Read(out int val8);
			msg.Read(out int val9);
			msg.Read(out int val10);
			msg.Read(out int val11);
			msg.Read(out int val12);
			msg.Read(out int val13);
			msg.Read(out long val14);
			list.Add(new ResultUnitZombie(val2, val3, val4, val5, val6, val7, val8, val9, val10, val11, val12, val13, val14));
            // Debug log for this client's data
            Debug.Log($"Info Recieced Player {i + 1}: isRed={val2}, Seq={val3}, Name={val4}, Kills={val5}, Deaths={val6}, Assists={val7}, " +
                      $"Score={val8}, Points={val9}, XP={val10}, Mission={val11}, PrevXP={val12}, NextXP={val13}, Buff={val14}");
        }
		list.Sort((ResultUnitZombie prev, ResultUnitZombie next) => prev.Compare(next));
		RoomManager.Instance.RU = list.ToArray();
		RoomManager.Instance.endCode = 0;
		RoomManager.Instance.redTotalKill = 0;
		RoomManager.Instance.redTotalDeath = 0;
		RoomManager.Instance.blueTotalKill = 0;
		RoomManager.Instance.blueTotalDeath = 0;
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnMatchEnd", (object)(sbyte)0);
			ClanMatchRounding component = gameObject.GetComponent<ClanMatchRounding>();
			if (null != component)
			{
				component.ShowRoundMessage = false;
			}
		}
	}

	private void HandleCS_ZOMBIE_INFECTION_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		for (int i = 0; i < val; i++)
		{
			msg.Read(out int val2);
			ZombieVsHumanManager.Instance.AddHuman(val2);
		}
		msg.Read(out int val3);
		for (int j = 0; j < val3; j++)
		{
			msg.Read(out int val4);
			ZombieVsHumanManager.Instance.AddZombie(val4);
		}
		msg.Read(out int val5);
		for (int k = 0; k < val5; k++)
		{
			msg.Read(out int val6);
			ZombieVsHumanManager.Instance.Die(val6);
		}
		msg.Read(out int val7);
		for (int l = 0; l < val7; l++)
		{
			msg.Read(out int val8);
			ZombieVsHumanManager.Instance.Die(val8);
		}
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnSelectZombies");
		}
	}

	private void HandleCS_ZOMBIE_STATUS_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		msg.Read(out int val3);
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnZombieStatus", new ZombieStatus(val, val2, val3), SendMessageOptions.DontRequireReceiver);
		}
	}

	private void HandleCS_ZOMBIE_BREAK_INTO_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		for (int i = 0; i < val; i++)
		{
			msg.Read(out int val2);
			ZombieVsHumanManager.Instance.AddHuman(val2);
		}
		msg.Read(out int val3);
		for (int j = 0; j < val3; j++)
		{
			msg.Read(out int val4);
			ZombieVsHumanManager.Instance.AddZombie(val4);
		}
		msg.Read(out int val5);
		for (int k = 0; k < val5; k++)
		{
			msg.Read(out int val6);
			ZombieVsHumanManager.Instance.Die(val6);
		}
		msg.Read(out int val7);
		for (int l = 0; l < val7; l++)
		{
			msg.Read(out int val8);
			ZombieVsHumanManager.Instance.Die(val8);
		}
	}

	private void HandleCS_ZOMBIE_INFECT_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		msg.Read(out int val2);
		ZombieVsHumanManager.Instance.AddZombie(val);
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnInfection", new Infection(val2, val));
			if (MyInfoManager.Instance.Seq == val)
			{
				Texture2D weaponByExcetion = GlobalVars.Instance.GetWeaponByExcetion(-11);
				BrickManDesc desc = BrickManManager.Instance.GetDesc(val2);
				if (desc != null)
				{
					gameObject.BroadcastMessage("OnKillLog", new KillInfo(desc.Nickname, MyInfoManager.Instance.Nickname, weaponByExcetion, null, desc.Seq, MyInfoManager.Instance.Seq, -11));
				}
			}
			else if (MyInfoManager.Instance.Seq == val2)
			{
				Texture2D weaponByExcetion2 = GlobalVars.Instance.GetWeaponByExcetion(-11);
				BrickManDesc desc2 = BrickManManager.Instance.GetDesc(val);
				if (desc2 != null)
				{
					gameObject.BroadcastMessage("OnKillLog", new KillInfo(MyInfoManager.Instance.Nickname, desc2.Nickname, weaponByExcetion2, null, MyInfoManager.Instance.Seq, desc2.Seq, -11));
				}
			}
			else
			{
				Texture2D weaponByExcetion3 = GlobalVars.Instance.GetWeaponByExcetion(-11);
				BrickManDesc desc3 = BrickManManager.Instance.GetDesc(val);
				BrickManDesc desc4 = BrickManManager.Instance.GetDesc(val2);
				if (desc3 != null && desc4 != null)
				{
					gameObject.BroadcastMessage("OnKillLog", new KillInfo(desc4.Nickname, desc3.Nickname, weaponByExcetion3, null, desc4.Seq, desc3.Seq, -11));
				}
			}
		}
	}

	private void HandleCS_SELECT_WANTED_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		WantedManager.Instance.AddWanted(val);
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.BroadcastMessage("OnSelectWanted", val, SendMessageOptions.DontRequireReceiver);
		}
		GameObject gameObject2 = BrickManManager.Instance.Get(val);
		if (null != gameObject2)
		{
			TPController component = gameObject2.GetComponent<TPController>();
			if (null != component)
			{
				component.BeginWanted();
			}
		}
	}

	private void HandleCS_KNOW_WANTED_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		for (int i = 0; i < val; i++)
		{
			msg.Read(out int val2);
			WantedManager.Instance.AddWanted(val2);
		}
	}

	private void HandleCS_DESELECT_WANTED_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		WantedManager.Instance.DelWanted(val);
	}

	private void HandleCS_ADD_AMMO_ACK(MsgBody msg)
	{
		GameObject gameObject = GameObject.Find("Me");
		if (null != gameObject)
		{
			EquipCoordinator component = gameObject.GetComponent<EquipCoordinator>();
			if (null != component)
			{
				component.AddAmmo(Weapon.TYPE.MAIN, 0.2f);
			}
		}
	}

	private void HandleCS_CLAN_APPLY_LIST_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		if (val > 0)
		{
			((ClanDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.CLAN))?.ClearClanReq();
		}
		for (int i = 0; i < val; i++)
		{
			msg.Read(out int val2);
			msg.Read(out string val3);
			msg.Read(out int val4);
			msg.Read(out int val5);
			msg.Read(out int val6);
			((ClanDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.CLAN))?.AddClanReq(val2, val3, val4, val5, val6);
		}
	}

	private void HandleCS_CLAN_CANCEL_APPLICATION_ACK(MsgBody msg)
	{
		((ClanDialog)DialogManager.Instance.GetDialog(DialogManager.DIALOG_INDEX.CLAN))?.DelClanReq();
	}

	private void HandleCS_CLAN_APPLICANT_COUNT_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		MyInfoManager.Instance.ClanApplicant = val;
	}

	private void HandleCS_CLAN_MATCHING_LIST_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		for (int i = 0; i < val; i++)
		{
			msg.Read(out int val2);
			msg.Read(out string val3);
			msg.Read(out string val4);
			msg.Read(out int val5);
			msg.Read(out int val6);
			msg.Read(out int val7);
			Room room = RoomManager.Instance.GetRoom(val2);
			if (room != null)
			{
				room.Title = val4;
				room.CurMapAlias = val3;
				room.Type = (Room.ROOM_TYPE)val5;
				room.weaponOption = val6;
				room.MaxPlayer = val7;
			}
		}
		GlobalVars.Instance.clanTeamMatchSuccess = -1;
	}

	private void HandleCS_CLAN_NEED_CREATE_POINT_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		GlobalVars.Instance.clanCreatePoint = val;
	}

	private void HandleCS_CLAN_CHANGE_ROOM_NAME_ACK(MsgBody msg)
	{
		msg.Read(out string val);
		Room room = RoomManager.Instance.GetRoom(RoomManager.Instance.CurrentRoom);
		if (room != null)
		{
			room.Title = val;
		}
	}

	private void HandleCS_CLAN_GET_OUT_SQUAD_ACK(MsgBody msg)
	{
		msg.Read(out int val);
		switch (val)
		{
		case -1:
			DialogManager.Instance.CloseAll();
			ContextMenuManager.Instance.CloseAll();
			P2PManager.Instance.Shutdown();
			CSNetManager.Instance.Sock.SendCS_LEAVE_REQ();
			CSNetManager.Instance.Sock.SendCS_LEAVE_SQUAD_REQ();
			SquadManager.Instance.Leave();
			CSNetManager.Instance.Sock.SendCS_LEAVE_SQUADING_REQ();
			SquadManager.Instance.Clear();
			GlobalVars.Instance.LobbyType = LOBBY_TYPE.ROOMS;
			GlobalVars.Instance.GotoLobbyRoomList = true;
			Application.LoadLevel("Lobby");
			break;
		case 0:
			GlobalVars.Instance.clanTeamMatchSuccess = -1;
			break;
		}
	}

	private string RemoveSystemKey(string msg)
	{
		msg = msg.Replace("\n", string.Empty);
		msg = msg.Replace("\t", string.Empty);
		msg = msg.Replace("[", " ");
		msg = msg.Replace("]", " ");
		return msg;
	}

	private string GetLoginFailString(int ret)
	{
		switch (ret)
		{
		case -1:
			return StringMgr.Instance.Get("NO_SUCH_ACCOUNT");
		case -2:
			return StringMgr.Instance.Get("INVALID_PASSWORD");
		case -3:
			return StringMgr.Instance.Get("DUPLICATE_LOGIN");
		case -4:
			return StringMgr.Instance.Get("INVALID_VERSION");
		case -111:
			return StringMgr.Instance.Get("SYSTEM_DEAD");
		case -115:
			return StringMgr.Instance.Get("WARNING_GM_BLOCK_USER");
		default:
			return StringMgr.Instance.Get("SYSTEM_ERROR");
		}
	}
}
