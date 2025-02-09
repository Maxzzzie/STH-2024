using System;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using System.Threading.Tasks;
using CitizenFX.Core.Native;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Mono.CSharp;

namespace STHMaxzzzie.Client
{
    public class SavePositions : BaseScript
    {
        //Savecalloutlocation
        [EventHandler("givePedLocationAndHeadingForDevMode")]
        private void givePedLocationAndHeadingForDevMode(int source, string locationName, string respawnOrCallout)
        {

            Ped playerPed = Game.PlayerPed;
            Vector3 position = playerPed.Position;
            float heading = playerPed.Heading;


            Vector4 playerLocation = new Vector4(position.X, position.Y, position.Z, heading);

            if (respawnOrCallout == "callout")
            {
                TriggerServerEvent("saveCallout", source, locationName, playerLocation);
            }
            else
            {
                TriggerServerEvent("saveRespawn", source, locationName, playerLocation);
            }
        }
    }

    public class Test : BaseScript
    {
        public List<string> soundList = new List<string>();

        public Test()
        {
            soundList.Add("10_SEC_WARNING,HUD_MINI_GAME_SOUNDSET");
            soundList.Add("10s,MP_MISSION_COUNTDOWN_SOUNDSET");
            soundList.Add("1st_Person_Transition,PLAYER_SWITCH_CUSTOM_SOUNDSET");
            soundList.Add("1st_Person_Transition,PLAYER_SWITCH_CUSTOM_SOUNDSET");
            soundList.Add("3_2_1,HUD_MINI_GAME_SOUNDSET");
            soundList.Add("3_2_1_NON_RACE,HUD_MINI_GAME_SOUNDSET");
            soundList.Add("5_SEC_WARNING,HUD_MINI_GAME_SOUNDSET");
            soundList.Add("5_Second_Timer,DLC_HEISTS_GENERAL_FRONTEND_SOUNDS");
            soundList.Add("5s,MP_MISSION_COUNTDOWN_SOUNDSET");
            soundList.Add("5s_To_Event_Start_Countdown,GTAO_FM_Events_Soundset");
            soundList.Add("ASSASSINATIONS_HOTEL_TIMER_COUNTDOWN,ASSASSINATION_MULTI");
            soundList.Add("ATM_WINDOW,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("Airhorn,DLC_TG_Running_Back_Sounds");
            soundList.Add("Apt_Style_Purchase,DLC_APT_Apartment_SoundSet");
            soundList.Add("Arming_Countdown,GTAO_Speed_Convoy_Soundset");
            soundList.Add("BACK,HUD_AMMO_SHOP_SOUNDSET");
            soundList.Add("BACK,HUD_FREEMODE_SOUNDSET");
            soundList.Add("BACK,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("BACK,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("BACK,HUD_FRONTEND_MP_SOUNDSET");
            soundList.Add("BACK,HUD_FRONTEND_TATTOO_SHOP_SOUNDSET");
            soundList.Add("BACK,HUD_MINI_GAME_SOUNDSET");
            soundList.Add("BASE_JUMP_PASSED,HUD_AWARDS");
            soundList.Add("BOATS_PLANES_HELIS_BOOM,MP_LOBBY_SOUNDS");
            soundList.Add("Beast_Checkpoint,APT_BvS_Soundset");
            soundList.Add("Beast_Checkpoint_NPC,APT_BvS_Soundset");
            soundList.Add("Bed,WastedSounds");
            soundList.Add("Beep_Green,DLC_HEIST_HACKING_SNAKE_SOUNDS");
            soundList.Add("Beep_Red,DLC_HEIST_HACKING_SNAKE_SOUNDS");
            soundList.Add("Blade_Appear,APT_BvS_Soundset");
            soundList.Add("Bomb_Disarmed,GTAO_Speed_Convoy_Soundset");
            soundList.Add("Boss_Blipped,GTAO_Magnate_Hunt_Boss_SoundSet");
            soundList.Add("Boss_Message_Orange,GTAO_Boss_Goons_FM_Soundset");
            soundList.Add("Breaker_01,DLC_HALLOWEEN_FVJ_Sounds");
            soundList.Add("Breaker_02,DLC_HALLOWEEN_FVJ_Sounds");
            soundList.Add("Bus_Schedule_Pickup,DLC_PRISON_BREAK_HEIST_SOUNDS");
            soundList.Add("CABLE_SNAPS,CONSTRUCTION_ACCIDENT_1_SOUNDS");
            soundList.Add("CAM_PAN_DARTS,HUD_MINI_GAME_SOUNDSET");
            soundList.Add("CANCEL,HUD_FREEMODE_SOUNDSET");
            soundList.Add("CANCEL,HUD_FRONTEND_CLOTHESSHOP_SOUNDSET");
            soundList.Add("CANCEL,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("CANCEL,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("CANCEL,HUD_LIQUOR_STORE_SOUNDSET");
            soundList.Add("CANCEL,HUD_MINI_GAME_SOUNDSET");
            soundList.Add("CAR_BIKE_WHOOSH,MP_LOBBY_SOUNDS");
            soundList.Add("CHALLENGE_UNLOCKED,HUD_AWARDS");
            soundList.Add("CHARACTER_SELECT,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("CHECKPOINT_AHEAD,HUD_MINI_GAME_SOUNDSET");
            soundList.Add("CHECKPOINT_BEHIND,HUD_MINI_GAME_SOUNDSET");
            soundList.Add("CHECKPOINT_MISSED,HUD_MINI_GAME_SOUNDSET");
            soundList.Add("CHECKPOINT_NORMAL,HUD_MINI_GAME_SOUNDSET");
            soundList.Add("CHECKPOINT_PERFECT,HUD_MINI_GAME_SOUNDSET");
            soundList.Add("CHECKPOINT_UNDER_THE_BRIDGE,HUD_MINI_GAME_SOUNDSET");
            soundList.Add("CLICK_BACK,WEB_NAVIGATION_SOUNDS_PHONE");
            soundList.Add("CLOSED,DLC_APT_YACHT_DOOR_SOUNDS");
            soundList.Add("CLOSED,MP_PROPERTIES_ELEVATOR_DOORS");
            soundList.Add("CLOSE_WINDOW,LESTER1A_SOUNDS");
            soundList.Add("CONFIRM_BEEP,HUD_MINI_GAME_SOUNDSET");
            soundList.Add("CONTINUE,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("CONTINUE,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("Camera_Shoot,Phone_Soundset_Franklin");
            soundList.Add("Checkpoint_Beast_Hit,FM_Events_Sasquatch_Sounds");
            soundList.Add("Checkpoint_Cash_Hit,GTAO_FM_Events_Soundset");
            soundList.Add("Checkpoint_Hit,GTAO_FM_Events_Soundset");
            soundList.Add("Checkpoint_Teammate,GTAO_Shepherd_Sounds");
            soundList.Add("Cheers,DLC_TG_Running_Back_Sounds");
            soundList.Add("Click,DLC_HEIST_HACKING_SNAKE_SOUNDS");
            soundList.Add("Click_Fail,WEB_NAVIGATION_SOUNDS_PHONE");
            soundList.Add("Click_Special,WEB_NAVIGATION_SOUNDS_PHONE");
            soundList.Add("Click_Special,WEB_NAVIGATION_SOUNDS_PHONE");
            soundList.Add("Continue_Accepted,DLC_HEIST_PLANNING_BOARD_SOUNDS");
            soundList.Add("Continue_Appears,DLC_HEIST_PLANNING_BOARD_SOUNDS");
             soundList.Add("Crash,DLC_HEIST_HACKING_SNAKE_SOUNDS");
            soundList.Add("Criminal_Damage_High_Value,GTAO_FM_Events_Soundset");
            soundList.Add("Criminal_Damage_Kill_Player,GTAO_FM_Events_Soundset");
            soundList.Add("Criminal_Damage_Low_Value,GTAO_FM_Events_Soundset");
            soundList.Add("Cycle_Item,DLC_Dmod_Prop_Editor_Sounds");
            soundList.Add("DELETE,HUD_DEATHMATCH_SOUNDSET");
            soundList.Add("Delete_Placed_Prop,DLC_Dmod_Prop_Editor_Sounds");
            soundList.Add("Deliver_Pick_Up,HUD_FRONTEND_MP_COLLECTABLE_SOUNDS");
            soundList.Add("DiggerRevOneShot,BulldozerDefault");
            soundList.Add("Door_Open,DOCKS_HEIST_FINALE_2B_SOUNDS");
               soundList.Add("Drill_Pin_Break,DLC_HEIST_FLEECA_SOUNDSET");
            soundList.Add("Dropped,HUD_FRONTEND_MP_COLLECTABLE_SOUNDS");
            soundList.Add("EDIT,HUD_DEATHMATCH_SOUNDSET");
                soundList.Add("ERROR,HUD_AMMO_SHOP_SOUNDSET");
            soundList.Add("ERROR,HUD_FRONTEND_CLOTHESSHOP_SOUNDSET");
            soundList.Add("ERROR,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("ERROR,HUD_FRONTEND_TATTOO_SHOP_SOUNDSET");
            soundList.Add("ERROR,HUD_LIQUOR_STORE_SOUNDSET");
            soundList.Add("EXIT,HUD_FRONTEND_DEFAULT_SOUNDSET");
              soundList.Add("End_Squelch,CB_RADIO_SFX");
            soundList.Add("Enemy_Capture_Start,GTAO_Magnate_Yacht_Attack_Soundset");
            soundList.Add("Enemy_Deliver,HUD_FRONTEND_MP_COLLECTABLE_SOUNDS");
            soundList.Add("Enemy_Pick_Up,HUD_FRONTEND_MP_COLLECTABLE_SOUNDS");
            soundList.Add("Enter_1st,GTAO_FM_Events_Soundset");
            soundList.Add("Enter_1st,GTAO_Magnate_Boss_Modes_Soundset");
            soundList.Add("Enter_Area,DLC_Lowrider_Relay_Race_Sounds");
            soundList.Add("Enter_Capture_Zone,DLC_Apartments_Drop_Zone_Sounds");
            soundList.Add("Event_Message_Purple,GTAO_FM_Events_Soundset");
            soundList.Add("Event_Start_Text,GTAO_FM_Events_Soundset");
            soundList.Add("Exit_Capture_Zone,DLC_Apartments_Drop_Zone_Sounds");
            soundList.Add("FAMILY_1_CAR_BREAKDOWN,FAMILY1_BOAT");
            soundList.Add("FAMILY_1_CAR_BREAKDOWN_ADDITIONAL,FAMILY1_BOAT");
            soundList.Add("FIRST_PLACE,HUD_MINI_GAME_SOUNDSET");
            soundList.Add("FLIGHT_SCHOOL_LESSON_PASSED,HUD_AWARDS");
            soundList.Add("FLYING_STREAM_END_INSTANT,FAMILY_5_SOUNDS");
            soundList.Add("Failure,DLC_HEIST_HACKING_SNAKE_SOUNDS");
            soundList.Add("Falling_Crates,EXILE_1");
            soundList.Add("Faster_Bar_Full,RESPAWN_ONLINE_SOUNDSET");
            soundList.Add("Faster_Click,RESPAWN_ONLINE_SOUNDSET");
              soundList.Add("FestiveGift,Feed_Message_Sounds");
            soundList.Add("FocusIn,HintCamSounds");
            soundList.Add("FocusOut,HintCamSounds");
                 soundList.Add("Friend_Deliver,HUD_FRONTEND_MP_COLLECTABLE_SOUNDS");
                 soundList.Add("Friend_Pick_Up,HUD_FRONTEND_MP_COLLECTABLE_SOUNDS");
            soundList.Add("Friend_Pick_Up,HUD_FRONTEND_MP_COLLECTABLE_SOUNDS");
            soundList.Add("Frontend_Beast_Fade_Screen,FM_Events_Sasquatch_Sounds");
            soundList.Add("Frontend_Beast_Freeze_Screen,FM_Events_Sasquatch_Sounds");
            soundList.Add("Frontend_Beast_Text_Hit,FM_Events_Sasquatch_Sounds");
            soundList.Add("Frontend_Beast_Transform_Back,FM_Events_Sasquatch_Sounds");
            soundList.Add("GO,HUD_MINI_GAME_SOUNDSET");
            soundList.Add("GO,HUD_MINI_GAME_SOUNDSET");
            soundList.Add("GOLF_BIRDIE,HUD_AWARDS");
            soundList.Add("GOLF_EAGLE,HUD_AWARDS");
            soundList.Add("GOLF_NEW_RECORD,HUD_AWARDS");
            soundList.Add("GO_NON_RACE,HUD_MINI_GAME_SOUNDSET");
               soundList.Add("Goal,DLC_HEIST_HACKING_SNAKE_SOUNDS");
               soundList.Add("Goon_Paid_Small,GTAO_Boss_Goons_FM_Soundset");
            soundList.Add("Grab_Parachute,BASEJUMPS_SOUNDS");
            soundList.Add("HIGHLIGHT,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("HIGHLIGHT_NAV_UP_DOWN,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("HOORAY,BARRY_02_SOUNDSET");
            soundList.Add("HORDE_COOL_DOWN_TIMER,HUD_FRONTEND_DEFAULT_SOUNDSET");
                soundList.Add("Hack_Failed,DLC_HEIST_BIOLAB_PREP_HACKING_SOUNDS");
                soundList.Add("Hack_Success,DLC_HEIST_BIOLAB_PREP_HACKING_SOUNDS");
            soundList.Add("Hang_Up,Phone_SoundSet_Michael");
            soundList.Add("Highlight_Accept,DLC_HEIST_PLANNING_BOARD_SOUNDS");
            soundList.Add("Highlight_Cancel,DLC_HEIST_PLANNING_BOARD_SOUNDS");
              soundList.Add("Highlight_Error,DLC_HEIST_PLANNING_BOARD_SOUNDS");
            soundList.Add("Highlight_Move,DLC_HEIST_PLANNING_BOARD_SOUNDS");
            soundList.Add("Hit,RESPAWN_ONLINE_SOUNDSET");
            soundList.Add("Hit,RESPAWN_SOUNDSET");
            soundList.Add("Hit,RESPAWN_SOUNDSET");
            soundList.Add("Hit_1,LONG_PLAYER_SWITCH_SOUNDS");
            soundList.Add("Hit_1,LONG_PLAYER_SWITCH_SOUNDS");
            soundList.Add("Hit_In,PLAYER_SWITCH_CUSTOM_SOUNDSET");
            soundList.Add("Hit_Out,PLAYER_SWITCH_CUSTOM_SOUNDSET");
            soundList.Add("Kill_List_Counter,GTAO_FM_Events_Soundset");
            soundList.Add("LEADERBOARD,HUD_MINI_GAME_SOUNDSET");
            soundList.Add("LEADER_BOARD,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("LIMIT,DLC_APT_YACHT_DOOR_SOUNDS");
            soundList.Add("LIMIT,GTAO_APT_DOOR_DOWNSTAIRS_GLASS_SOUNDS");
            soundList.Add("LIMIT,GTAO_APT_DOOR_DOWNSTAIRS_WOOD_SOUNDS");
                soundList.Add("LOCAL_PLYR_CASH_COUNTER_COMPLETE,DLC_HEISTS_GENERAL_FRONTEND_SOUNDS");
                 soundList.Add("LOCAL_PLYR_CASH_COUNTER_INCREASE,DLC_HEISTS_GENERAL_FRONTEND_SOUNDS");
            soundList.Add("LOOSE_MATCH,HUD_MINI_GAME_SOUNDSET");
              soundList.Add("LOSER,HUD_AWARDS");
            soundList.Add("Lester_Laugh_Phone,DLC_HEIST_HACKING_SNAKE_SOUNDS");
            soundList.Add("Lights_On,GTAO_MUGSHOT_ROOM_SOUNDS");
            soundList.Add("Load_Scene,DLC_Dmod_Prop_Editor_Sounds");
            soundList.Add("Lose_1st,GTAO_FM_Events_Soundset");
            soundList.Add("Lose_1st,GTAO_Magnate_Boss_Modes_Soundset");
            soundList.Add("Lowrider_Upgrade,Lowrider_Super_Mod_Garage_Sounds");
            soundList.Add("MARKER_ERASE,HEIST_BULLETIN_BOARD_SOUNDSET");
            soundList.Add("MEDAL_BRONZE,HUD_AWARDS");
            soundList.Add("MEDAL_GOLD,HUD_AWARDS");
            soundList.Add("MEDAL_SILVER,HUD_AWARDS");
            soundList.Add("MEDAL_UP,HUD_MINI_GAME_SOUNDSET");
            soundList.Add("MICHAEL_LONG_SCREAM,FAMILY_5_SOUNDS");
            soundList.Add("MP_5_SECOND_TIMER,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("MP_AWARD,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("MP_Flash,WastedSounds");
            soundList.Add("MP_IDLE_KICK,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("MP_IDLE_TIMER,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("MP_Impact,WastedSounds");
            soundList.Add("MP_RANK_UP,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("MP_WAVE_COMPLETE,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("Map_Roll_Down,DLC_HEIST_PLANNING_BOARD_SOUNDS");
            soundList.Add("Map_Roll_Up,DLC_HEIST_PLANNING_BOARD_SOUNDS");
            soundList.Add("Menu_Accept,Phone_SoundSet_Default");
            soundList.Add("Mission_Pass_Notify,DLC_HEISTS_GENERAL_FRONTEND_SOUNDS");
            soundList.Add("Mission_Pass_Notify,DLC_HEISTS_GENERAL_FRONTEND_SOUNDS");
            soundList.Add("NAV,HUD_AMMO_SHOP_SOUNDSET");
              soundList.Add("NAV_LEFT_RIGHT,HUD_FREEMODE_SOUNDSET");
               soundList.Add("NAV_LEFT_RIGHT,HUD_FRONTEND_DEFAULT_SOUNDSET");
                soundList.Add("NAV_LEFT_RIGHT,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("NAV_UP_DOWN,HUD_FREEMODE_SOUNDSET");
            soundList.Add("NAV_UP_DOWN,HUD_FRONTEND_CLOTHESSHOP_SOUNDSET");
            soundList.Add("NAV_UP_DOWN,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("NAV_UP_DOWN,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("NAV_UP_DOWN,HUD_FRONTEND_TATTOO_SHOP_SOUNDSET");
            soundList.Add("NAV_UP_DOWN,HUD_LIQUOR_STORE_SOUNDSET");
            soundList.Add("NAV_UP_DOWN,HUD_MINI_GAME_SOUNDSET");
            soundList.Add("NO,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("Nav_Arrow_Ahead,DLC_HEISTS_GENERAL_FRONTEND_SOUNDS");
            soundList.Add("Nav_Arrow_Behind,DLC_HEISTS_GENERAL_FRONTEND_SOUNDS");
                soundList.Add("Nav_Arrow_Left,DLC_HEISTS_GENERAL_FRONTEND_SOUNDS");
            soundList.Add("Nav_Arrow_Right,DLC_HEISTS_GENERAL_FRONTEND_SOUNDS");
            soundList.Add("Near_Miss_Counter_Reset,GTAO_FM_Events_Soundset");
            soundList.Add("OFF,v_4");
            soundList.Add("OK,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("ON,NOIR_FILTER_SOUNDS");
            soundList.Add("ON,v_4");
            soundList.Add("OOB_Cancel,GTAO_FM_Events_Soundset");
            soundList.Add("OOB_Start,GTAO_FM_Events_Soundset");
            soundList.Add("OPENED,MP_PROPERTIES_ELEVATOR_DOORS");
            soundList.Add("OPEN_WINDOW,LESTER1A_SOUNDS");
            soundList.Add("OTHER_TEXT,HUD_AWARDS");
            soundList.Add("Object_Collect_Player,GTAO_FM_Events_Soundset");
            soundList.Add("Object_Collect_Remote,GTAO_FM_Events_Soundset");
            soundList.Add("Object_Dropped_Remote,GTAO_FM_Events_Soundset");
            soundList.Add("Off_High,MP_RADIO_SFX");
            soundList.Add("On_Call_Player_Join,DLC_HEISTS_GENERAL_FRONTEND_SOUNDS");
            soundList.Add("Oneshot_Final,MP_MISSION_COUNTDOWN_SOUNDSET");
            soundList.Add("Out_Of_Area,DLC_Lowrider_Relay_Race_Sounds");
            soundList.Add("PERSON_SCROLL,HEIST_BULLETIN_BOARD_SOUNDSET");
            soundList.Add("PERSON_SELECT,HEIST_BULLETIN_BOARD_SOUNDSET");
                 soundList.Add("PICKUP_WEAPON_SMOKEGRENADE,HUD_FRONTEND_WEAPONS_PICKUPS_SOUNDSET");
            soundList.Add("PICK_UP,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("PICK_UP_WEAPON,HUD_FRONTEND_CUSTOM_SOUNDSET");
            soundList.Add("PIN_BUTTON,ATM_SOUNDS");
            soundList.Add("PIPES_LAND,CONSTRUCTION_ACCIDENT_1_SOUNDS");
            soundList.Add("PROPERTY_PURCHASE,HUD_AWARDS");
            soundList.Add("PROPERTY_PURCHASE_MEDIUM,HUD_PROPERTY_SOUNDSET");
            soundList.Add("PS2A_MONEY_LOST,PALETO_SCORE_2A_BANK_SS");
            soundList.Add("PURCHASE,HUD_FRONTEND_TATTOO_SHOP_SOUNDSET");
            soundList.Add("PURCHASE,HUD_LIQUOR_STORE_SOUNDSET");
            soundList.Add("PUSH,DLC_APT_YACHT_DOOR_SOUNDS");
            soundList.Add("PUSH,GTAO_APT_DOOR_DOWNSTAIRS_GLASS_SOUNDS");
            soundList.Add("PUSH,GTAO_APT_DOOR_DOWNSTAIRS_WOOD_SOUNDS");
            soundList.Add("Paper_Shuffle,DLC_HEIST_PLANNING_BOARD_SOUNDS");
            soundList.Add("Parcel_Vehicle_Lost,GTAO_FM_Events_Soundset");
            soundList.Add("Payment_Non_Player,DLC_HEISTS_GENERIC_SOUNDS");
            soundList.Add("Payment_Player,DLC_HEISTS_GENERIC_SOUNDS");
            soundList.Add("Pen_Tick,DLC_HEIST_PLANNING_BOARD_SOUNDS");
            soundList.Add("Phone_Generic_Key_02,HUD_MINIGAME_SOUNDSET");
            soundList.Add("Phone_Generic_Key_03,HUD_MINIGAME_SOUNDSET");
            soundList.Add("Pin_Bad,DLC_HEIST_BIOLAB_PREP_HACKING_SOUNDS");
                 soundList.Add("Pin_Centred,DLC_HEIST_BIOLAB_PREP_HACKING_SOUNDS");
            soundList.Add("Pin_Good,DLC_HEIST_BIOLAB_PREP_HACKING_SOUNDS");
            soundList.Add("Place_Prop_Fail,DLC_Dmod_Prop_Editor_Sounds");
            soundList.Add("Place_Prop_Success,DLC_Dmod_Prop_Editor_Sounds");
            soundList.Add("Player_Collect,DLC_PILOT_MP_HUD_SOUNDS");
            soundList.Add("Player_Enter_Line,GTAO_FM_Cross_The_Line_Soundset");
            soundList.Add("Player_Exit_Line,GTAO_FM_Cross_The_Line_Soundset");
            soundList.Add("Power_Down,DLC_HEIST_HACKING_SNAKE_SOUNDS");
            soundList.Add("Pre_Screen_Stinger,DLC_HEISTS_FAILED_SCREEN_SOUNDS");
            soundList.Add("Pre_Screen_Stinger,DLC_HEISTS_FINALE_SCREEN_SOUNDS");
            soundList.Add("Pre_Screen_Stinger,DLC_HEISTS_FINALE_SCREEN_SOUNDS");
            soundList.Add("Pre_Screen_Stinger,DLC_HEISTS_PREP_SCREEN_SOUNDS");
            soundList.Add("Pre_Screen_Stinger,DLC_HEISTS_PREP_SCREEN_SOUNDS");
            soundList.Add("Put_Away,Phone_SoundSet_Michael");
            soundList.Add("QUIT,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("QUIT_WHOOSH,HUD_MINI_GAME_SOUNDSET");
            soundList.Add("RACE_PLACED,HUD_AWARDS");
            soundList.Add("RAMP_DOWN,TRUCK_RAMP_DOWN");
            soundList.Add("RAMP_UP,TRUCK_RAMP_DOWN");
            soundList.Add("RANK_UP,HUD_AWARDS");
            soundList.Add("REMOTE_PLYR_CASH_COUNTER_COMPLETE,DLC_HEISTS_GENERAL_FRONTEND_SOUNDS");
            soundList.Add("REMOTE_PLYR_CASH_COUNTER_INCREASE,DLC_HEISTS_GENERAL_FRONTEND_SOUNDS");
            soundList.Add("RESTART,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("RETRY,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("ROBBERY_MONEY_TOTAL,HUD_FRONTEND_CUSTOM_SOUNDSET");
            soundList.Add("ROPE_CUT,ROPE_CUT_SOUNDSET");
            soundList.Add("ROUND_ENDING_STINGER_CUSTOM,CELEBRATION_SOUNDSET");
            soundList.Add("Radar_Beast_Blip,FM_Events_Sasquatch_Sounds");
            soundList.Add("Reset_Prop_Position,DLC_Dmod_Prop_Editor_Sounds");
            soundList.Add("Retune_High,MP_RADIO_SFX");
            soundList.Add("SCREEN_FLASH,CELEBRATION_SOUNDSET");
            soundList.Add("SELECT,HUD_FREEMODE_SOUNDSET");
            soundList.Add("SELECT,HUD_FRONTEND_CLOTHESSHOP_SOUNDSET");
            soundList.Add("SELECT,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("SELECT,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("SELECT,HUD_FRONTEND_MP_SOUNDSET");
            soundList.Add("SELECT,HUD_FRONTEND_MP_SOUNDSET");
            soundList.Add("SELECT,HUD_FRONTEND_TATTOO_SHOP_SOUNDSET");
            soundList.Add("SELECT,HUD_LIQUOR_STORE_SOUNDSET");
            soundList.Add("SELECT,HUD_MINI_GAME_SOUNDSET");
            soundList.Add("SHOOTING_RANGE_ROUND_OVER,HUD_AWARDS");
            soundList.Add("SKIP,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("STUN_COLLECT,MINUTE_MAN_01_SOUNDSET");
            soundList.Add("SWING_SHUT,GTAO_APT_DOOR_DOWNSTAIRS_GLASS_SOUNDS");
            soundList.Add("SWING_SHUT,GTAO_APT_DOOR_DOWNSTAIRS_WOOD_SOUNDS");
            soundList.Add("Save_Scene,DLC_Dmod_Prop_Editor_Sounds");
            soundList.Add("ScreenFlash,MissionFailedSounds");
            soundList.Add("ScreenFlash,WastedSounds");
            soundList.Add("Select_Placed_Prop,DLC_Dmod_Prop_Editor_Sounds");
            soundList.Add("Shard_Disappear,GTAO_Boss_Goons_FM_Shard_Sounds");
            soundList.Add("Shard_Disappear,GTAO_FM_Events_Soundset");
            soundList.Add("Short_Transition_In,PLAYER_SWITCH_CUSTOM_SOUNDSET");
            soundList.Add("Short_Transition_Out,PLAYER_SWITCH_CUSTOM_SOUNDSET");
            soundList.Add("Start,DLC_HEIST_HACKING_SNAKE_SOUNDS");
            soundList.Add("Start_Squelch,CB_RADIO_SFX");
            soundList.Add("Success,DLC_HEIST_HACKING_SNAKE_SOUNDS");
            soundList.Add("Sult-16_Super_Mod_Garage_Sounds,1");
            soundList.Add("Swap_Sides,DLC_HALLOWEEN_FVJ_Sounds");
            soundList.Add("TENNIS_MATCH_POINT,HUD_AWARDS");
            soundList.Add("TENNIS_POINT_WON,HUD_AWARDS");
            soundList.Add("TIMER,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("TIMER_STOP,HUD_MINI_GAME_SOUNDSET");
            soundList.Add("TOGGLE_ON,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("TOGGLE_ON,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("TRAFFIC_CONTROL_MOVE_CROSSHAIR,BIG_SCORE_3A_SOUNDS");
            soundList.Add("Tattooing_Oneshot,TATTOOIST_SOUNDS");
            soundList.Add("Tattooing_Oneshot_Remove,TATTOOIST_SOUNDS");
            soundList.Add("Team_Capture_Start,GTAO_Magnate_Yacht_Attack_Soundset");
            soundList.Add("TextHit,WastedSounds");
            soundList.Add("Thermal_Off,CAR_STEAL_2_SOUNDSET");
            soundList.Add("Thermal_On,CAR_STEAL_2_SOUNDSET");
            soundList.Add("Timer_10s,DLC_HALLOWEEN_FVJ_Sounds");
            soundList.Add("Traffic_Control_Fail,BIG_SCORE_3A_SOUNDS");
            soundList.Add("Traffic_Control_Fail_Blank,BIG_SCORE_3A_SOUNDS");
            soundList.Add("Traffic_Control_Light_Switch_Back,BIG_SCORE_3A_SOUNDS");
            soundList.Add("Turn,DLC_HEIST_HACKING_SNAKE_SOUNDS");
            soundList.Add("UNDER_THE_BRIDGE,HUD_AWARDS");
            soundList.Add("UNDER_WATER_COME_UP,0");
            soundList.Add("UNDO,HEIST_BULLETIN_BOARD_SOUNDSET");
            soundList.Add("WAYPOINT_SET,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("WEAKEN,CONSTRUCTION_ACCIDENT_1_SOUNDS");
            soundList.Add("WEAPON_AMMO_PURCHASE,HUD_AMMO_SHOP_SOUNDSET");
            soundList.Add("WEAPON_ATTACHMENT_EQUIP,HUD_AMMO_SHOP_SOUNDSET");
            soundList.Add("WEAPON_ATTACHMENT_UNEQUIP,HUD_AMMO_SHOP_SOUNDSET");
            soundList.Add("WEAPON_PURCHASE,HUD_AMMO_SHOP_SOUNDSET");
            soundList.Add("WEAPON_SELECT_ARMOR,HUD_AMMO_SHOP_SOUNDSET");
            soundList.Add("WEAPON_SELECT_BATON,HUD_AMMO_SHOP_SOUNDSET");
            soundList.Add("WEAPON_SELECT_FUEL_CAN,HUD_AMMO_SHOP_SOUNDSET");
            soundList.Add("WEAPON_SELECT_GRENADE_LAUNCHER,HUD_AMMO_SHOP_SOUNDSET");
            soundList.Add("WEAPON_SELECT_HANDGUN,HUD_AMMO_SHOP_SOUNDSET");
            soundList.Add("WEAPON_SELECT_KNIFE,HUD_AMMO_SHOP_SOUNDSET");
            soundList.Add("WEAPON_SELECT_OTHER,HUD_AMMO_SHOP_SOUNDSET");
            soundList.Add("WEAPON_SELECT_PARACHUTE,HUD_AMMO_SHOP_SOUNDSET");
            soundList.Add("WEAPON_SELECT_RIFLE,HUD_AMMO_SHOP_SOUNDSET");
            soundList.Add("WEAPON_SELECT_RPG_LAUNCHER,HUD_AMMO_SHOP_SOUNDSET");
            soundList.Add("WEAPON_SELECT_SHOTGUN,HUD_AMMO_SHOP_SOUNDSET");
                soundList.Add("WIN,HUD_AWARDS");
            soundList.Add("Whistle,DLC_TG_Running_Back_Sounds");
            soundList.Add("Whoosh_1s_L_to_R,MP_LOBBY_SOUNDS");
            soundList.Add("Whoosh_1s_R_to_L,MP_LOBBY_SOUNDS");
            soundList.Add("YES,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("Zone_Enemy_Capture,DLC_Apartments_Drop_Zone_Sounds");
            soundList.Add("Zone_Neutral,DLC_Apartments_Drop_Zone_Sounds");
            soundList.Add("Zone_Team_Capture,DLC_Apartments_Drop_Zone_Sounds");
            soundList.Add("Zoom_In,DLC_HEIST_PLANNING_BOARD_SOUNDS");
            soundList.Add("Zoom_Left,DLC_HEIST_PLANNING_BOARD_SOUNDS");
            soundList.Add("Zoom_Out,DLC_HEIST_PLANNING_BOARD_SOUNDS");
            soundList.Add("Zoom_Right,DLC_HEIST_PLANNING_BOARD_SOUNDS");
            soundList.Add("CHECKPOINT_MISSED,HUD_MINI_GAME_SOUNDSET");
            soundList.Add("CHECKPOINT_NORMAL,HUD_MINI_GAME_SOUNDSET");
            soundList.Add("Knuckle_Crack_Hard_Cel,MP_SNACKS_SOUNDSET");
            soundList.Add("Knuckle_Crack_Slap_Cel,MP_SNACKS_SOUNDSET");
            soundList.Add("Slow_Clap_Cel,MP_SNACKS_SOUNDSET");
            soundList.Add("CONTINUOUS_SLIDER,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("SAFE_DOOR_CLOSE,SAFE_CRACK_SOUNDSET");
            soundList.Add("SAFE_DOOR_OPEN,SAFE_CRACK_SOUNDSET");
            soundList.Add("TUMBLER_PIN_FALL,SAFE_CRACK_SOUNDSET");
            soundList.Add("TUMBLER_PIN_FALL_FINAL,SAFE_CRACK_SOUNDSET");
            soundList.Add("TUMBLER_RESET,SAFE_CRACK_SOUNDSET");
            soundList.Add("TUMBLER_TURN,SAFE_CRACK_SOUNDSET");
            soundList.Add("Hit_In,PLAYER_SWITCH_CUSTOM_SOUNDSET");
            soundList.Add("Camera_Move_Loop,PLAYER_SWITCH_CUSTOM_SOUNDSET");
            soundList.Add("Hit_Out,PLAYER_SWITCH_CUSTOM_SOUNDSET");
            soundList.Add("Change_Cam,MP_CCTV_SOUNDSET");
            soundList.Add("Background,MP_CCTV_SOUNDSET");
            soundList.Add("Change_Cam,MP_CCTV_SOUNDSET");
            soundList.Add("Background,DLC_HEIST_HACKING_SNAKE_SOUNDS");
            soundList.Add("Trail_Custom,DLC_HEIST_HACKING_SNAKE_SOUNDS");
            soundList.Add("Return_To_Vehicle_Timer,GTAO_FM_Events_Soundset");
            soundList.Add("Timer_10s,GTAO_FM_Events_Soundset");
                soundList.Add("TextHit,MissionFailedSounds");
            soundList.Add("Bed,MissionFailedSounds");
            soundList.Add("10S,MP_MISSION_COUNTDOWN_SOUNDSET");
            soundList.Add("Altitude_Warning,EXILE_1");
            soundList.Add("DISTANT_DOG_BARK,CAR_STEAL_2_SOUNDSET");
            soundList.Add("Short_Transition_In,PLAYER_SWITCH_CUSTOM_SOUNDSET");
            soundList.Add("In,SHORT_PLAYER_SWITCH_SOUND_SET");
            soundList.Add("Altitude_Warning,EXILE_1");
            soundList.Add("WIND,CONSTRUCTION_ACCIDENT_1_SOUNDS");
            soundList.Add("Elevation_Loop,DLC_Dmod_Prop_Editor_Sounds");
            soundList.Add("Move_Loop,DLC_Dmod_Prop_Editor_Sounds");
            soundList.Add("Rotate_Loop,DLC_Dmod_Prop_Editor_Sounds");
            soundList.Add("Background,MP_CCTV_SOUNDSET");
            soundList.Add("Pan,MP_CCTV_SOUNDSET");
            soundList.Add("Zoom,MP_CCTV_SOUNDSET");
            soundList.Add("Change_Cam,MP_CCTV_SOUNDSET");
            soundList.Add("In,SHORT_PLAYER_SWITCH_SOUND_SET");
            soundList.Add("Short_Transition_In,PLAYER_SWITCH_CUSTOM_SOUNDSET");
            soundList.Add("Short_Transition_In,PLAYER_SWITCH_CUSTOM_SOUNDSET");
            soundList.Add("Short_Transition_In,PLAYER_SWITCH_CUSTOM_SOUNDSET");
            soundList.Add("PICKUP_WEAPON_BALL,HUD_FRONTEND_WEAPONS_PICKUPS_SOUNDSET");
            soundList.Add("10s,MP_MISSION_COUNTDOWN_SOUNDSET");
            soundList.Add("Pickup_Briefcase,GTAO_Magnate_Boss_Modes_Soundset");
            soundList.Add("CLOSING,MP_PROPERTIES_ELEVATOR_DOORS");
            soundList.Add("OPENING,MP_PROPERTIES_ELEVATOR_DOORS");
            soundList.Add("DOOR_BUZZ,MP_PLAYER_APARTMENT");
                soundList.Add("NO,HUD_FRONTEND_DEFAULT_SOUNDSET");
                  soundList.Add("QUIT,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("YES,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("Hit_Out,PLAYER_SWITCH_CUSTOM_SOUNDSET");
            soundList.Add("Short_Transition_In,PLAYER_SWITCH_CUSTOM_SOUNDSET");
            soundList.Add("Grab_Chute_Foley,DLC_Pilot_Chase_Parachute_Sounds");
            soundList.Add("Pull_Out,Phone_SoundSet_Franklin");
            soundList.Add("OPENED,DOOR_GARAGE");
            soundList.Add("OPENING,DOOR_GARAGE");
            soundList.Add("Thernal_Vision_Loop,APT_BvS_Soundset");
            soundList.Add("Pickup_Briefcase,GTAO_Magnate_Boss_Modes_Soundset");
            soundList.Add("Blip_Pickup,GTAO_Magnate_Boss_Modes_Soundset");
            soundList.Add("Altitude_Warning,EXILE_1");
            soundList.Add("Background_Loop,CB_RADIO_SFX");
            soundList.Add("1st_Person_Transition,PLAYER_SWITCH_CUSTOM_SOUNDSET");
            soundList.Add("HIT_OUT,PLAYER_SWITCH_CUSTOM_SOUNDSET");
            soundList.Add("Hit_In,PLAYER_SWITCH_CUSTOM_SOUNDSET");
                soundList.Add("Short_Transition_In,PLAYER_SWITCH_CUSTOM_SOUNDSET");
               soundList.Add("EMP,DLC_HALLOWEEN_FVJ_Sounds");
                  soundList.Add("EMP_Blast,DLC_HEISTS_BIOLAB_FINALE_SOUNDS");
            soundList.Add("10S,MP_MISSION_COUNTDOWN_SOUNDSET");
            soundList.Add("5S,MP_MISSION_COUNTDOWN_SOUNDSET");
            soundList.Add("Timer_10s,DLC_TG_Dinner_Sounds");
            soundList.Add("Timer_5s,DLC_TG_Dinner_Sounds");
            soundList.Add("Out_of_Bounds,MP_MISSION_COUNTDOWN_SOUNDSET");
            soundList.Add("OOB_Timer_Dynamic,GTAO_FM_Events_Soundset");
            soundList.Add("LIMIT,DLC_APT_YACHT_DOOR_SOUNDS");
            soundList.Add("LIMIT,GTAO_APT_DOOR_DOWNSTAIRS_GLASS_SOUNDS");
            soundList.Add("PUSH,DLC_APT_YACHT_DOOR_SOUNDS");
            soundList.Add("PUSH,GTAO_APT_DOOR_DOWNSTAIRS_GLASS_SOUNDS");
            soundList.Add("CONTINUOUS_SLIDER,HUD_FRONTEND_DEFAULT_SOUNDSET");
            soundList.Add("Camera_Zoom,Phone_SoundSet_Glasses_Cam");
            soundList.Add("DOOR_BUZZ,MP_PLAYER_APARTMENT");
            soundList.Add("Camera_Shoot,Phone_SoundSet_Glasses_Cam");
            soundList.Add("Background_Sound,Phone_SoundSet_Glasses_Cam");
            soundList.Add("ROBBERY_MONEY_TOTAL,HUD_FRONTEND_CUSTOM_SOUNDSET");
            soundList.Add("10s,MP_MISSION_COUNTDOWN_SOUNDSET");
            soundList.Add("Out_of_Bounds,MP_MISSION_COUNTDOWN_SOUNDSET");
            soundList.Add("Pin_Movement,DLC_HEIST_BIOLAB_PREP_HACKING_SOUNDS");
            soundList.Add("5s,MP_MISSION_COUNTDOWN_SOUNDSET");
            soundList.Add("Pin_Movement,DLC_HEIST_BIOLAB_PREP_HACKING_SOUNDS");
            soundList.Add("ZOOM,PAPARAZZO_02_SOUNDSETS");
            soundList.Add("Phone_Generic_Key_02,HUD_MINIGAME_SOUNDSET");
            soundList.Add("Phone_Generic_Key_03,HUD_MINIGAME_SOUNDSET");
            soundList.Add("all,SHORT_PLAYER_SWITCH_SOUND_SET");
            soundList.Add("Change_Station_Loud,Radio_Soundset");
            soundList.Add("TRAFFIC_CONTROL_BG_NOISE,BIG_SCORE_3A_SOUNDS");
            soundList.Add("TRAFFIC_CONTROL_CHANGE_CAM,BIG_SCORE_3A_SOUNDS");
            soundList.Add("TRAFFIC_CONTROL_TOGGLE_LIGHT,BIG_SCORE_3A_SOUNDS");
            soundList.Add("ROBBERY_MONEY_TOTAL,HUD_FRONTEND_CUSTOM_SOUNDSET");
            soundList.Add("FINDING_VIRUS,LESTER1A_SOUNDS");
            soundList.Add("OOB_Timer_Dynamic,GTAO_FM_Events_Soundset");
            soundList.Add("Pin_Movement,DLC_HEIST_BIOLAB_PREP_HACKING_SOUNDS");
            soundList.Add("Short_Transition_In,PLAYER_SWITCH_CUSTOM_SOUNDSET");
            soundList.Add("Camera_Hum,BIG_SCORE_SETUP_SOUNDS");
            soundList.Add("Camera_Zoom,BIG_SCORE_SETUP_SOUNDS");
            soundList.Add("10_SEC_WARNING,HUD_MINI_GAME_SOUNDSET");
            soundList.Add("out,SHORT_PLAYER_SWITCH_SOUND_SET");
            soundList.Add("LIMIT,GTAO_APT_DOOR_DOWNSTAIRS_GLASS_SOUNDS");
            soundList.Add("LIMIT,GTAO_APT_DOOR_DOWNSTAIRS_WOOD_SOUNDS");
            soundList.Add("PUSH,GTAO_APT_DOOR_DOWNSTAIRS_GLASS_SOUNDS");
            soundList.Add("PUSH,GTAO_APT_DOOR_DOWNSTAIRS_WOOD_SOUNDS");
            soundList.Add("All,SHORT_PLAYER_SWITCH_SOUND_SET");
            soundList.Add("SwitchRedWarning,SPECIAL_ABILITY_SOUNDSET");
             soundList.Add("SwitchWhiteWarning,SPECIAL_ABILITY_SOUNDSET");
            soundList.Add("Heli_Crash,FBI_HEIST_FINALE_CHOPPER");
            soundList.Add("Found_Target,POLICE_CHOPPER_CAM_SOUNDS");
                 soundList.Add("Lost_Target,POLICE_CHOPPER_CAM_SOUNDS");
            soundList.Add("Microphone,POLICE_CHOPPER_CAM_SOUNDS");
            soundList.Add(",TIMER_STOP_MASTER");
                 soundList.Add("SIGN_DESTROYED,HUD_AWARDS");
                 soundList.Add("Remote_Enemy_Enter_Line,GTAO_FM_Cross_The_Line_Soundset");
            soundList.Add("Remote_Friendly_Enter_Line,GTAO_FM_Cross_The_Line_Soundset");
            soundList.Add("Lose_1st,GTAO_Magnate_Boss_Modes_Soundset");
            soundList.Add("Enter_1st,GTAO_Magnate_Boss_Modes_Soundset");
            soundList.Add("CHECKPOINT_UNDER_THE_BRIDGE,HUD_MINI_GAME_SOUNDSET");
            soundList.Add("Near_Miss_Counter,GTAO_FM_Events_Soundset");
            soundList.Add("Goon_Paid_Large,GTAO_Boss_Goons_FM_Soundset");
            soundList.Add("COLLECTED,HUD_AWARDS");
            soundList.Add("PEYOTE_COMPLETED,HUD_AWARDS");
        }

        [EventHandler("clientTest1")]
        async void clientTest1()
        {
            // foreach (string sound in soundList)
            // {

            //     string[] splitSound = sound.Split(',');
            //     Debug.WriteLine(splitSound[0] + "," + splitSound[1]);
            //     NotificationScript.ShowSpecialNotification($"{splitSound[0]} {splitSound[1]}", splitSound[0], splitSound[1]);
            //     await Delay(6000);
            //     StopSound(500);
            // }
            PlaySoundFrontend(-1, "Super_Mod_Garage_Upgrade_Car_Default", "0", true);
            Debug.WriteLine($"1");
            await Delay(2000);
            PlaySoundFrontend(-1, "car_crushed", "dlc_vw_body_disposal_sounds", true);
            Debug.WriteLine($"2");
            await Delay(2000);
            PlaySoundFrontend(-1, "Checkpoint_Finish", "DLC_AW_Frontend_Sounds", false);
            Debug.WriteLine($"3");
            await Delay(2000);
            PlaySoundFrontend(-1, "Banshee2_Upgrade", "JA16_Super_Mod_Garage_Sounds", true);
            Debug.WriteLine($"4");
            await Delay(2000);
            PlaySoundFrontend(-1, "Faction3_Upgrade", "Low2_Super_Mod_Garage_Sounds", true);
            Debug.WriteLine($"5");
            await Delay(2000);
            PlaySoundFrontend(-1, "Lowrider_Upgrade", "Lowrider_Super_Mod_Garage_Sounds", true);
            Debug.WriteLine($"6");
            await Delay(2000);
            PlaySoundFrontend(-1, "Purchase_Upgrade", "DLC_EXEC_Warehouse_Upgrade_Bench_Sounds", true);
            Debug.WriteLine($"7");
            await Delay(2000);
            PlaySoundFrontend(-1, "SultanRS_Upgrade", "JA16_Super_Mod_Garage_Sounds", true);
            Debug.WriteLine($"8");
            await Delay(2000);
            PlaySoundFrontend(-1, "Enter_On_Foot", "GTAO_ImpExp_Enter_Exit_Garage_Sounds", true);
            Debug.WriteLine($"9");
            await Delay(2000);
            PlaySoundFrontend(-1, "Empty_Fire_Fail", "DLC_AW_Machine_Gun_Ammo_Counter_Sounds", true);
            Debug.WriteLine($"10");
            await Delay(2000);
            PlaySoundFrontend(-1, "flares_empty", "DLC_SM_Countermeasures_Sounds", true);
            Debug.WriteLine($"11");
            await Delay(2000);
            PlaySoundFrontend(-1, "rc_mines_empty", "DLC_AW_RCBandito_Mine_Sounds", true);
            Debug.WriteLine($"12");
            await Delay(2000);
            PlaySoundFrontend(-1, "inactive_fire_fail", "dlc_xm_orbital_cannon_sounds", true);
            Debug.WriteLine($"13");
            await Delay(2000);
            PlaySoundFrontend(-1, "shotgun_shard", "dlc_hei4_hidden_collectibles_sounds", true);
            Debug.WriteLine($"14");
            await Delay(2000);
            PlaySoundFrontend(-1,"Transform_Local_Player", "DLC_Exec_TP_SoundSet", false);
            Debug.WriteLine($"15");
            await Delay(2000);
            PlaySoundFrontend(-1, "HOORAY", "BARRY_02_SOUNDSET", true);
            Debug.WriteLine($"16");
            await Delay(2000);


        }

        [EventHandler("testAllVehiclesFrom")]
        async static void testAllVehiclesFrom(int value)
        {
            if (value > VehicleHandler.vehicleinfoDict.Count)
            {
                NotificationScript.ShowErrorNotification($"[TEST] cannot run testallvehicles. Value was wrong.");
            }
            if (value < 0) value = 0;

            int testTo = value + 10;
            if (testTo > VehicleHandler.vehicleinfoDict.Count) testTo = VehicleHandler.vehicleinfoDict.Count;
            while (value < testTo)
            {
                var kvp = VehicleHandler.vehicleinfoDict.ElementAt(value);
                NotificationScript.ShowNotification($"processing({value}) - {kvp.Value}");
                VehicleHandler.SetPlayerIntoNewVehicle(kvp.Key);
                value++;
                string[] info = kvp.Value.ToString().Split(',');
                if (bool.TryParse(info[3].ToString(), out bool restriction)) await Delay(6000); //if vehicle has a restriction.
                else { NotificationScript.ShowNotification($"~r~No ~b~restriction ~q~set"); await Delay(20000); }
            }
            NotificationScript.ShowNotification($"Testing done.");
        }

        async static Task<bool> DoesVehicleNameExist(string vehName)
        {
            Vehicle spawned = await World.CreateVehicle(new Model(Game.GenerateHash(vehName)), new Vector3(0, 0, 0), 0);
            if (spawned == null) return false;
            spawned.Delete();
            return true;
        }

       
    }
}
