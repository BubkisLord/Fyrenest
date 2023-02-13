global using System;
global using System.IO;
global using System.Collections;
global using Modding;
global using UnityEngine;
global using SFCore;
global using System.Collections.Generic;
global using System.Linq;
using ItemChanger;
using ItemChanger.Tags;
using ItemChanger.UIDefs;
using Satchel.BetterMenus;
using GlobalEnums;
using ItemChanger.Internal;
using ItemChanger.Modules;
using UnityEngine.SceneManagement;
using static Fyrenest.Fyrenest;

namespace Fyrenest
{
    public class TextReplacements
    {
        public static readonly TextReplacements instance = new();
        public TextReplacements() {}

        #region LanguageReplacements
        public string LanguageGet(string key, string sheetTitle, string orig)
        {
            if (key == "VOIDSOUL_NAME")
            {
                return "Void Soul";
            }

            if (key == "XERO_TALK")
            {
                return "Stop there! Those who try to leave this kingdom are cursed, like those who forget the light. Better to stay stuck here, until time itself sleeps.<page>Hope and freedom. I thought that strength alone could grant me these things. In my dreams I would cut that dark with my nails, but then I turned my weapons upon the light.<page>When I awoke in this place, I began to understand. Those who hope for freedom are already doomed, as are those who forget that they do not have it.<page>Do you still hope, wanderer? Will you obey the light? Or will you raise your weapon and doom us both?";
            }

            if (key == "MAGE_LORD")
            {
                return "In my dreams I could see it. The Kingdom's salvation, the fight against the light... the answer was in the soul that animates our bodies.<page>There was a darkness there... It could have saved us all...<page>But why?! He opposed everything I did...<page>His jealousy... his madness... he believed he could embody the light itself!<page>But he will never be able to. He will never have enough of the energy... The energy which resides in all living things...<page>Some call it a purified version of soul, some call it fragments of the light itself. But, some others... others who truly understand... They call it...<page>Essence.";
            }

            if (key == "GALIEN_DEFEAT")
            {
                return "Unbelievable... I am defeated at last! So this is what it feels like to be bested...<page>Still though, I am strong, am I not? When you see our master, surely you'll tell him of my valour...?<page>Yes... he sent you here to test me, didn't he? I knew he had not forgotten brave Galien.<page>I am ready... to finally be free. Finally... free of those chains...<page>...My sentence is over... Let me be free...<page>...please...";
            }

            if (key == "DESC_MANTIS_LORD")
            {
                return "Leaders of the Mantis tribe and its finest warriors. In legend, they were said to fight a god-like being of immense power, and its misguided followers. One among them was a mysterious bug who hides away somewhere in Fyrenest, who is now the last of the followers.";
            }

            if (key == "HU_DEFEAT" && sheetTitle == "Ghosts")
            {
                return "My mind... it clears. Have we been... sleeping, child?<page>Aah... I remember. Our leader. He drove us to ruin. Destroyed this world. Attacked the rest of us.<page>Killed.<page>Killed... me...?<page>...aah. I see now...";
            }

            if (key == "KCIN_REPEAT")
            {
                return "Seeking power is a hollow goal. Best to search for other things. Trust me, our old leader sought power. It destroyed us all.";
            }

            if (key == "DISTANT_VILLAGE" && sheetTitle == "Map Zones")
            {
                return "Herrah's Den";
            }

            if (key == "BONE_FOREST" && sheetTitle == "Map Zones")
            {
                return "Bone Forest";
            }

            if (key == "TEST_AREA" && sheetTitle == "Map Zones")
            {
                return "Test area, please tell me where this is. Ping me @BubkisLord#5187 (discord)";
            }

            if (key == "DREAM_WORLD" && sheetTitle == "Map Zones")
            {
                return "Dream World";
            }

            if (key == "ELDERBUG_FLOWER" && sheetTitle == "Prompts")
            {
                return "Give the Elderbug a flower for literal no reason?";
            }

            if (key == "CARD" && sheetTitle == "Cornifer")
            {
                return "Hi there, it's me! Conifer. Clearly, I have left, but you can come at see me or Pine at dirtmouth!";
            }

            if (key == "ELDERBUG_INTRO_VISITEDCROSSROAD" && sheetTitle == "Elderbug")
            {
                return "My, my, look at that! I haven't talked to someone in so long! How great to have you! Welcome to Dirtmouth.";
            }

            if (key == "ELDERBUG_DREAM" && sheetTitle == "Elderbug")
            {
                return "Hello? Is someone there?<page>Who is that? Aah! What was that? That feeling...<page>...Like the cold, terrifying embrace of death...";
            }

            if (key == "BELIEVE_TAB_50" && sheetTitle == "Backer Messages")
            {
                return "Good job on completing the game with Fyrenest on! What should I update? What new features would you like to see next? What could I improve on? Tell me on the Discord Hollow Knight Modding Server!";
            }

            if (key == "BANKER_DREAM_SPA" && sheetTitle == "Banker")
            {
                return "Ahh! Leave me be, you stout little knight! What grossness!";
            }

            if (key == "BANKER_DEPOSIT" && sheetTitle == "Banker")
            {
                return "Thank you! I will keep your hard earned geo safe! Hee hee heeeee!";
            }

            if (key == "BANKER_BALANCE_ZERO_REPEAT" && sheetTitle == "Banker")
            {
                return "Please! Please don't hurt me! I have no geo left!";
            }

            if (key == "BANKER_SPA_REPEAT" && sheetTitle == "Banker")
            {
                return "We're still friends right? Remember all those great times together! All the banking!";
            }
            if (key == "CP2" && sheetTitle == "GRIMMSYCOPHANT_INSPECT")
            {
                return "...Raw energy... ...The troupe...<page>Why?";
            }
            if (key == "CP2" && sheetTitle == "GRIMMSYCOPHANT_DREAM")
            {
                return "...I founded the troupe... ...Destroyed kingdoms... ...For Grimm...";
            }
            if (key == "CROSSROADS_SUB" && sheetTitle == "Titles")
            {
                return "Of Flame";
            }
            if (key == "CROSSROADS_SUB_INF" && sheetTitle == "Titles")
            {
                return "Of Flame";
            }
            if (key == "CORNIFER_SUB" && sheetTitle == "Titles")
            {
                return "The Adventurer";
            }
            if (orig.Contains("Hollow Knight"))
            {
                return orig.Replace("Hollow Knight", "Infected Vessel");
            }
            if (orig.Contains("#!#"))
            {
                return orig.Replace("#!#", "");
            }
            if (orig.Contains("Pure Vessel"))
            {
                return orig.Replace("Pure Vessel", "Hollow Vessel");
            }
            if (orig.Contains("The Fading Town"))
            {
                return orig.Replace("The Fading Town", "The Realm of The Old One");
            }
            if (orig.Contains("Elderbug"))
            {
                return orig.Replace("Elderbug", "The Old One");
            }
            if (orig.Contains("Dirtmouth"))
            {
                return orig.Replace("Dirtmouth", "Fyrecamp");
            }
            if (orig.Contains("Deepnest"))
            {
                return orig.Replace("Deepnest", "Deepfyre");
            }
            if (orig.Contains("Iselda"))
            {
                return orig.Replace("Iselda", "Pine");
            }
            if (orig.Contains("Cornifer"))
            {
                return orig.Replace("Conifer", "Conifer");
            }
            if (orig.Contains("Hallownest"))
            {
                return orig.Replace("Hallownest", "Fyrenest");
            }
            if (orig.Contains("Howling Cliffs"))
            {
                return orig.Replace("Howling Cliffs", "Inferno's Peak");
            }
            if (orig.Contains("Fungal Wastes"))
            {
                return orig.Replace("Fungal Wastes", "Defiled Wastelands");
            }
            if (orig.Contains("Kingdom's Edge"))
            {
                return orig.Replace("Kingdom's Edge", "Fyre's Edge");
            }
            if (orig.Contains("Ancient Basin"))
            {
                return orig.Replace("Ancient Basin", "Void Basin");
            }
            if (orig.Contains("Abyss"))
            {
                return orig.Replace("Abyss", "Pit");
            }
            if (orig.Contains("Resting Grounds"))
            {
                return orig.Replace("Resting Grounds", "Spirit Sanctuary");
            }
            if (orig.Contains("Crystal Peak"))
            {
                return orig.Replace("Crystal Peak", "Crystalline Mountain");
            }
            if (orig.Contains("City of Tears"))
            {
                return orig.Replace("City of Tears", "City of Flame");
            }
            if (SlyDeal.instance.Equipped() && sheetTitle == "Prices")
            {
                try
                {
                    float numOrig = float.Parse(orig);
                    numOrig /= 1.5f;
                    orig = numOrig.ToString();
                    return orig;
                }
                catch (Exception)
                {
                    return orig;
                }
            }
            if (ZoteBorn.instance.Equipped() && sheetTitle == "Prices")
            {
                return "10000";
            }
            if (SlyDeal.instance.Equipped() && sheetTitle == "Elderbug")
            {
                return "I don't think I can speak to you right now. There is a disgusting whiff of something coming by...";
            }
            if (SlyDeal.instance.Equipped() && sheetTitle == "Iselda")
            {
                return "Ugh... Please, leave.. The smell!<page>Just buy something. Or don't. Please leave the shop.";
            }
            if (SlyDeal.instance.Equipped() && sheetTitle == "Enemy Dreams")
            {
                return "...That stench... ...disgusting...";
            }
            if (SlyDeal.instance.Equipped() && sheetTitle == "Sly")
            {
                return "I see you are wearing that charm I gave you... Not near me please.";
            }
            if (SlyDeal.instance.Equipped() && sheetTitle == "Cornifer")
            {
                return "You can have a map! Have everything! But get me away from that stench!";
            }
            if (sheetTitle == "Lore Tablets" && key == "WISHING_WELL_INSPECT")
            {
                return "A true follower of the Pale King gives all they own for the kingdom.";
            }
            if (sheetTitle == "Lore Tablets" && key == "TUT_TAB_02")
            {
                return "For those who enter Fyrenest from the far lands of Hallownest and The Glimmering Realm, note this. Beyond this point you enter the land of the light. Step across this threshold and obey our laws. Bear witness to one of the last civilisations, one of the eternal Kingdoms.\n\nFyrenest.";
            }
            if (sheetTitle == "Lore Tablets" && key == "RANDOM_POEM_STUFF")
            {
                return "Fyrenest is great.\nThe very last great kingdom.\nFyrenest rules all.\n                                                 - Haiku by Monomon the Teacher.";
            }
            if (sheetTitle == "Lore Tablets" && key == "RUINS_FOUNTAIN")
            {
                return "Memorial to the Hollow Vessel.\n\nIn its vault far above, stopping the death.\n\nThrough its sacrifice Fyrenest will last eternal.";
            }
            if (sheetTitle == "Cornifer" && key == "FUNGAL_WASTES_GREET")
            {
                return "Ahh my short friend, you've caught me at the perfect time. I'm impressed that you have got this far! I'm just about finished charting these noxious caverns. Very territorial types make their homes within this area. I'd suggest avoiding this place. I don't think it would be very safe for a fragile little one like you. I have heard of a group of deadly warriors, they seemed an intelligent bunch. I wouldn't go down there if I were you.";
            }
            if (sheetTitle == "Cornifer" && key == "GREENPATH_GREET")
            {
                return "Oh, hello there! I didn't think you would be here! You are surely having someone's help traversing this path... Surely, someone of your small stature couldn't get around like this! Buy a map, it will help you to get back to Fyrecamp. You are clearly lost for some reason.";
            }
            if (sheetTitle == "Cornifer" && key == "MINES_GREET")
            {
                return "Hello, my short little friend! What a suprise finding you here! Have you come to scale the mountains? I'm afraid you are much to pathetic to do that. Here, buy a map instead! It might help you find a way out.";
            }
            if (sheetTitle == "Cornifer" && key == "CROSSROADS_GREET")
            {
                return "Hello again! Still winding your way through these twisting highways? Just imagine how they must have looked during the kingdom's prime, thick with traffic and bustling with life! I wish I could have seen it. Oh, it is a shame that our old king is gone... Would you like to buy a map of the area to help you get out safely? You don't seem the adventurous type.";
            }
            if (sheetTitle == "Cornifer" && key == "CLIFFS_GREET")
            {
                return "Are you enjoying the bracing air? I doubt you have experienced something like this before, since you are always scrounging around underground, looking for geo like a hermit. Anyway, we are quite close to the borders of Fyrenest, and the desolate plains that surround it. I have heard that these plains make bugs go mad... Seeking escape, only to find lost memories and distant towns. I have heard of a place far away, where our king went when he left us. A place called Hallownest, a distant kingdom never to be found... I had a brother named Cornifer, he left in search of that horrid place... I dread what has happened to him... But, lingering on the past doesn't accomplish anything. I've drawn out a small map for the area, although simple, it is helpful nonetheless. Not knowing the full extents of a region can be quite frustrating.";
            }
            if (orig.ToLower().Contains("the world of infected vessel"))
            {
                return orig.Replace("Infected Vessel", "Hollow Knight");
            }
            if (key == "WITCH_REWARD_8A")
            {
                return "Yes. The time has come.<page>The Dream Nail... And you as well, Wielder. It is time for you both to awaken.<page>The Essence you have collected... Finally, I will be able to re-enact the second stage of my plan. Pure potential! Let the power course through you and into the Dream Nail!<page>Hold it aloft, Wielder!<page>AWAKEN!";
            }
            if (key == "WITCH_FINAL_1")
            {
                return "So much Essence... Finally. So bright! I will be able to retake this land.<page>You see, the folk of my tribe were born from a light. Light similar to Essence, similar to that powerful blade, though much brighter still.<page>They were content to bask in that light and honoured it. Worshipped it. For a time...<br>But we lost our way. Forgot our traditions...<page>But another light appeared in our world... A wyrm that took the form of a king. He was born here, from the remanents of Fyrenest's essence and light. Fyrenest's power was forever destroyed, absorbed by a narcissistic king.<page>How fickle my ancestors must have been. They forsook the light that spawned them. Turned their backs to it... Forgot it even.<page>I have rectified their mistakes. I will ascend to a might never seen before. All those champions you slew, all those warriors you killed, I have been harvesting their power. Did you see the prison I set their spirits in? A eternal fighting ring.<page>You have been collecting essence since you came here. Going out and fetching more for me. Now I have enough to ascend. Ascension is my final goal. You see, I am the light! I am the blinding radiance Fyrenest needs! Everyone will worship me for the end of time.<page>No one shall forget me. No one shall lose their way. I will be their god! Their ruler!";
            }
            if (key == "WITCH_FINAL_2")
            {
                return "Finally. I ascend! The time has come!\n\nI WILL RULE ALL OF FYRENEST! NOTHING SHALL STOP ME!<page>I SHALL BE ETERNAL, I SHALL BE THE LIGHT!<page>It was a mistake to give all your essence to me. Nevertheless, I am thankful. A new kingdom will be born.<page>A kingdom built upon betrayal.";
            }
            if (key == "WITCH_FINAL_3")
            {
                return "IT IS HAPPENING!\nI AM ASCENDING!<page>SO BRIGHT!<page>I SEE THE LIGHT! THE RADIANCE!<page>ME.";
            }
            if (key == "WITCH_MEET_A")
            {
                return "Ah, welcome back to the waking world. Those Dreamers. They inhabit every civilisation wherever you go. They are nothing, just ignore them.";
            }
            if (key == "WITCH_REWARD_5")
            {
                return "So, you already have 700 Essence. Take your gift and continue collecting Essence for me.<page>I need more if I want to ascend. Just know that once you have 900, you must come back and visit me.";
            }
            if (key == "WITCH_REWARD_7")
            {
                return "So, you already have 1500 Essence. Soon you will gain a special gift.<page>Continue on your path, Wielder. I know not what guides you, nor what will it is that drives you forward. But know, if that drive disappears, you will pay dearly. Once you have collected 1800 Essence though, I will be here waiting.<page>Take this gift, may it grant you strength and help you to collect the Essence of this world!";
            }
            if (key == "WITCH_REWARD_4")
            {
                return "Ahhhhh. 500 Essence. You're a master in the making. Well done! Well done! I've a small reward for you. Don't let it get in your head though, we still have a long way to go. I need more.<page>Plucked from one of my most precious memories, this Charm will bring you and the Dream Nail closer together still. The secrets of this kingdom won't be able to hide from you any longer!<page>Take it, and return once you have collected 700 Essence. More gifts await you... and me...";
            }
            if (key == "WITCH_GENERIC")
            {
                return "Explore Fyrenest and collect Essence. There are beings of great power that harbor extreme power. Seek them out, reveal them, and gather the light inside...";
            }
            if (key == "WITCH_MEET_B")
            {
                return "Ahhhh, you've found your way back to me. When you awoke you just left. You made a good choice coming back here. I have a use for your services.";
            }
            if (key == "WITCH_MEET_B")
            {
                return "Ahhhh, you've found your way back to me. When you awoke you just left. You made a good choice coming back here. I have a use for your services.";
            }
            if (key == "WITCH_REWARD_8B")
            {
                return "Ahhhh, ah ha ha ha ha, yes...<page>No dream can hide itself from you now. You can peer into the darkest places... You just need to find the right crack.<page>What will you do with such a power, Wielder? Whose memories will you hunt down?<page>Hah. Do as you wish, once my plan is fulfilled. Find the last remaining scraps of Essence. Seek it out. Find it, and bring it to me. I want it all.";
            }
            if (key == "WITCH_INTRO")
            {
                return "Those figures, those Dreamers... they reached out with what little power they still have and dragged you into that hidden place. They feel threatened by you. They were weak.<page>They couldn't do what needed to be done.<br>Let's see if you fare better.<page>Wait, that talisman you now wield, the Dream Nail... it can cut through the veil that separates the waking world from our dreams. Maybe you shall fare better.<page>Though I must admit, that sacred blade has dulled over time. Together perhaps, we can restore its power. You only have to bring me Essence. Yes... That works.<page>Essence... they are precious fragments of light and energy collected from dreams. Collect it wherever you find any, and bring it to me. Once we have enough, we can work wonders together. We will re-create all of Fyrenest!<page>Go out into the world, Wielder. Hunt down the Essence that lingers there!<page>Collect 100 Essence and return to me. I will teach you more... The ways of ascension.";
            }
            if (key == "HINT_WITCH_DREAMPLANT")
            {
                return "Essence can be found wherever dreams take root.<page>Have you seen them? Those whispering plants that grow all over this old Kingdom? I believe there is one just outside. Why not strike it with your Dream Nail, and see what happens? Collect my Essence.";
            }
            if (key == "WITCH_REWARD_1")
            {
                return "Hmm, already you've collected 100 Essence. Quick work! Things come naturally to you, don't they?<page>No wonder the Dreamers tried to bury you in that old dream. Perhaps being prisoners themselves, they desired your company?<page>In any case, now their prison is better sealed and their Essence is still being harvested. Do not worry about those Dreamers. They won't bother us anymore. Take this old trinket as encouragement from me, and return when you have collected 200 Essence.";
            }
            if (key == "WITCH_REWARD_6")
            {
                return "The Dream Nail glows bright... It holds over 1200 Essence. Looking into it I can see so many memories peering back at me. So many asking to be remembered.<page>None of us can live forever, and so we ask those who survive to remember us.<page>Hold something in your mind and it lives on with you, but forget it and you seal it away forever. That is the only death that matters.<page>Huh, so they say! Enough of that though. Take this relic and come back to me with 1500 Essence. Go, get me more Essence!";
            }
            if (key == "WITCH_REWARD_2A")
            {
                return "Ahhh... Your Dream Nail holds over 200 Essence. You're proving your talent in its collection.<page>Have you seen that great door just outside? My tribe closed it long ago and forbade its opening.<page>But, since I want more essence, I am going to open it. The spirits of strong beings you have killed will be imprisoned there. While imprisoned, I can harvest their energy, giving me even more Essence.<page>You can also visit, and battle them over and over. Every victory gives me more Essence.";
            }
            for (int i = 1; i < 10; i++)
            {
                if (key == "WITCH_QUEST_" + i.ToString())
                {
                    return "You still require more essence before I give you another reward.";
                }
            }
            if (key == "WITCH_REWARD_2B")
            {
                return "There we go! The door to the prison is open! Go ahead and fight them if you want. Just know, the Essence will go straight to me, not your dream nail. And I won't reward you for prowess in combat against them.";
            }
            if (key == "WITCH_QUEST_5B")
            {
                return "My, my, look at you! Once you collect 900 Essence, I will teach you something hidden for a very long time. You are going to like it.";
            }
            if (key == "WITCH_GREET")
            {
                return "Ah, Wielder, you've returned. Let me have a look at the Dream Nail...";
            }
            if (key == "WITCH_REWARD_5B")
            {
                return "The Dream Nail now holds 900 Essence within its core!<page>Yes, you're starting to see them. The connections between us and the dreams we leave behind, like prints in the dust. The time has come for you to learn how to revisit the places connected to you!<page>Hold the Dream Nail tight, wielder, and imagine a great gate opening before you!";
            }
            if (key == "WITCH_HINT_XERO")
            {
                return "Sometimes people can be infused with Essence. Some of the former members of our tribe conducted experiments on themselves. I created an experiment on a willing tribe member, and it turned out exactly the way it was designed. I am the only one who knows how to do an Essence infusion successfully. One day, with enough Essence, I will do it on myself.<page>Although they are bountiful sources of Essence, instead of slaying them we banished them across the kingdom.<page>You should search carefully near graves and other monuments. Why, I believe I saw an interesting gravestone here in the Resting Grounds.<page>If you do decide to disturb those dreams though, be prepared for a fight... Their spirits are easily angered.";
            }
            if (key == "WITCH_DREAM1")
            {
                return "What a terrible fate they've visited upon you.<page>To cast you away into this space between body and soul.<page>Will you accept their judgement and fade slowly away?<page>Or will you take the weapon before you, and cut your way out of this sad, forgotten dream?";
            }
            if (key == "WITCH_DREAM_FALL")
            {
                return "Though you may fall, your will shall carry you forward.<page>A dream is endless, but a Kingdom is not.<page>The power to wake this world from its slumber... Only worshipping the light will restore that.";
            }
            if (key == "WITCH_DREAM")
            {
                return "Once I get the Essence, I will be unstoppable. I will re-create Fyrenest in my own image. People will worship the light once more. Wait. I sense...<br>Get out of my mind. I trusted you with a powerful weapon such as this, and you disobey me? GET OUT!";
            }
            if (key == "HORNET_PRE_FINAL_BATTLE")
            {
                return "You have destroyed the Dreamers. The path is open, but are you sure this is the right one?\nI will aid you in the coming battle, but I fear that there is something else.<page>Something feels wrong, but I trust your judgement, little knight.";
            }
            if (key == "HORNET_PRE_FINAL_BATTLE_DREAM")
            {
                return "This is not the final battle. This is not the enemy. Something is wrong...";
            }
            if (key == "HORNET_GREENPATH")
            {
                return "Come no closer, ghost.<page>I've seen you, creeping in the undergrowth, stalking me.\nI do not know who you are, but you are not my enemy. I know what you will do. What everyone does. They side with the hidden power.<page>You must be destroyed.";
            }
            if (key == "HORNET_SPIDER_TOWN_REPEAT")
            {
                return "Leave me now, ghost. Allow me a moment alone before this bedchamber forever becomes a shrine.<page>Although, I feel that this is not the way...";
            }
            if (key == "HORNET_DOOR_UNOPENED")
            {
                return "I'm impressed little ghost. You've burdened yourself with the fate of this world, yet you still stand strong.<page>To stop the seer would alone be considered an impossible task, but to accept that void inside yourself, that casts you as something rather exceptional.";
            }
            if (key == "HORNET_OUTSKIRTS_2")
            {
                return "Prove yourself to be strong enough, prove that you can defeat the dreams, lest they become nightmares.";
            }
            if (key == "HORNET_FOUNTAIN_1")
            {
                return "Again we meet little ghost.<page>I'm normally quite perceptive. You I underestimated, though I've since guessed the truth.<page>It's no surprise you've managed to reach the heart of this world. In so doing, you shall know the sacrifice that keeps it standing.<page>A great sacrifice to hold the light. To keep it at bay.";
            }
            if (key == "HORNET_FOUNTAIN_2")
            {
                return "If, knowing that truth, you'd still attempt a role in Fyrenest's perpetuation. Do not fail.";
            }
            if (key == "HORNET_ABYSS_ASCENT_01")
            {
                return "Ghost. I see you've faced the place of your birth, and now drape yourself in the substance of its shadow.<page>Though our strength is born of similar source, that part of you, that crucial emptiness, I do not share.<page>Funny then, that such darkness gives me hope. Within it, I see the chance of change.<page>A difficult journey you would face, but a choice it can create. Aid the light, let it rise, or snuff it out.";
            }
            if (key == "HORNET_SPIDER_TOWN_01")
            {
                return "So you've slain the Beast... and you head towards that fated goal.<page>I'd not have obstructed this happening, but it caused me some pain to knowingly stand idle.<page>...What? You might think me stern but I'm not completely cold.<page>We do not choose our mothers, or the circumstance into which we are born. Despite all the ills of this world, I'm thankful for the life she granted me.<page>It's quite a debt I owed. Only in allowing her to pass, and taking the burden of the future in her stead, can I begin to repay it.";
            }
            if (key == "HORNET_OUTSKIRTS_DEFEAT")
            {
                return "...So strong... You could do it, if you had the will.<page>But could you raise your nail once knowing its tragic conception? And knowing yourself?...<page>Then do it, Ghost of Fyrenest! Head onward. Take the artifact which unites void and light.";
            }
            if (key == "ELDERBUG_DREAM")
            {
                return "If only the rest of Dirtmouth was full once more.";
            }
            if (key == "ELDERBUG_GENERIC_2")
            {
                return "I'm a bit bored of talking right now, come back later. There's only so much I can handle!";
            }
            if (key == "ELDERBUG_HISTORY_1")
            {
                return "Many used to come from far and wide, hoping the kingdom would fulfill their desires.<page>Fyrenest. Supposedly the greatest kingdom there ever was, full of treasures and secrets.<page>But sadly, now it's nothing more than a poisonous tomb full of dreams and decay.<page>Everything fades eventually, I suppose.<page>Hmm, that reminds me of story I heard long ago about an area of Fyrenest that had faded. The color sucked out of the stone and brick.";
            }
            if (key == "ELDERBUG_INTRO_MAIN")
            {
                return "The other residents, they've all disappeared. Headed down that well, one by one, into the caverns below.<page>Used to be there was a great kingdom beneath our town. It's long fell to ruin, yet it still draws folks into its depths.<page>Wealth, glory, enlightenment, power, that darkness seems to promise all things. I'm sure you too seek your dreams down there. My dreams are long gone. Taken, by those who crave them.<page>Well watch out. It's a sickly air that fills the place. Creatures turn mad and travellers are robbed of their memories, hopes and dreams.<page>Perhaps dreams aren't such great things after all...";
            }
            if (key == "ELDERBUG_TEMPLE_VISITED")
            {
                return "Did you visit that temple? A strange building I've heard, though I'd never dare the journey myself.<page>The braver among us once went there to pray, said they felt peace and power radiating from within the walls. After a while, they stopped going.<page>It became a very empty and morbid place. Many stopped praying and worshipping. Now it serves as a forgotten shrine.<page>I wonder what changed?";
            }
            if (key == "MANTIS_PLAQUE_01")
            {
                return "To all hidden dreams, welcome.<br>May you find swift end upon our claws.";
            }
            if (key == "STAG_CROSSROADS")
            {
                return "Although from a time lost past, I remember when these highways and crossroads pulsed with life. These are the paths where I carried my first passengers through when I was young.<page>Now those travellers are gone. Maybe it was better when we worshipped light and power above. Shunning that power came with a great cost.";
            }
            if (key == "STAG_ROYALGARDENS")
            {
                return "I'd almost forgotten this station existed. It was not often used by the common bugs, being a well-guarded retreat for our late Queen.<page>Even as overgrown as they've become, these gardens are still beautiful after all this time. I'm sure the Queen would be happy to know that.<page>Ironic, isn't it? The Queen was the bug who first had the idea to stop worshipping the light. Now, she is one of the only ones still free. Some could say it is a blessing to be free, but when alone, it feels like a curse. Sometimes I want to just let go, and let the power take me.";
            }
            if (key == "STAG_DEEPNEST")
            {
                return "Hmm, throughout all these years, I have never been to this area. It feels cold, deep down, deeper than anywhere else in Fyrenest. Secluded, far away from the ruins of civilisation.";
            }
            if (key == "")
            {
                return "";
            }
            if (key == "")
            {
                return "";
            }
            if (key == "")
            {
                return "";
            }
            if (key == "")
            {
                return "";
            }
            if (key == "")
            {
                return "";
            }
            if (key == "")
            {
                return "";
            }
            if (key == "")
            {
                return "";
            }
            if (key == "")
            {
                return "";
            }
            return orig;
        }
        #endregion

        public void Hook()
        {
            ModHooks.LanguageGetHook += LanguageGet;
        }
    }
}