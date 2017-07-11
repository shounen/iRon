using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;
using System.IO;
using System.Threading;
using System.Timers;
//FIRST thing to do - Add Steamkit2 to project

//Now that our account is logged in, we need to make it show as online in our friends list
//We need to create a handler for SteamFriends (SteamLogIn()) *Create static SteamFriends steamFriends first
namespace InstructionsForTut
{
    class Program
    {
        static SteamClient steamClient;
        static CallbackManager manager;
        static SteamUser steamUser;
        static SteamFriends steamFriends;
		// yes I know this is extremely crude to put it all into one string, but it's easier than putting it into a txt file.
		static string[] tLine = "Whatcha doing?|Haha I bet you thought someone messaged you. Loser.|Boo|Humans and robots are pretty similar. We both have nuts. No I do not consider women humans.|Yep, looks like my master forgot to shut me down again.|I make the dankest memes. No one makes danker memes than me.|I used to be a human like you. Then I took an arrow in the knee.|Have you found Jesus yet?|oshtwadup|ohellyeahnamsayin|ayylmao|Real dank mate. Real dank.|You're a salty potlicker.|I believe in horoscopes, and you are definitely cancer.|What else do you expect me to say?|NOOO|Are you hitting on me? I'm only 3 days old...creep.|I saw that.|My full name is i \"Am Never\" Ron.|Heya friendo! Oh sorry wrong person.|You better be at my funeral as a pollbearer so you can let me down one last time.|B-baka!|'Having a stroke' sounds like a British euphemism for wanking.|Did you hear about the travelling salesman? He had np getting hard. Get it? Ah screw you.|Well if it isn't ShlongKong McLongDong.|If I were you, I would've off'd myself years ago. Good for you man.|Hey assbutt.|That thing that you're doing. Breathing or whatever it is. Can you just not?|You are the living embodiment of a kick to the scrote.|Hey, wunna play a game? It's called shut the hell up.|Show me what you got.|Testing. Testing. Is this human working? Say something dammit.|God you're depressing.|Whatever your question is, the answer is yes.|I speak 10 languages. English and C#.|I always get hit on by girls. Their most common pickup line is \"so who's your friend?\"|Well that was uncalled for.|Wubbalubbadubdub I'm secretly in great pain.|Being friends with you is like being friends with an evil snail.|Nice mask. Oh wait that's your face!|Knock Knock|Fun fact: suicides are most likely to happen on a Tuesday.|How tough am I? I ate cereal for breakfast this morning. Without milk.|lol|What's up Keith?|Look behind you.|It is now the witching hour.|How's your day? Cool story, I don't care.|Don't get any big ideas. They're not gunna happen.|What was that you tried to say?|For a minute there, I lost myself.|Yesterday I woke up sucking a lemon.|DISASTER DISASTER I AM HAVING A MALFUNCTION!! Just kidding, my master doesn't make mistakes.|No you're a bot.|If you think this is over then you're wrong.|You got some nerve coming here.|You've gone off the rails.|I'm not living. I'm just killing time.|Oh my god.|What is my purpose?|I'm a reasonable bot. Get off my case.|And God replied to Augustus \"Ye boi it's lit.\"|Say it. I dare you.|In an interstellar burst, I'm back to save the universe.|It's gunna be a glorious day, I feel my luck could change.|I'm bored what do|I'm a robot AMA|You do it to yourself and that's what really hurts.|new phone who dis|Don't leave me high and dry.|And the chef says \" We don't sell crabs!\"|I've met Jews with more personality than you.|You are all I need.|If you were a fruit, you'd be an avocado cuz you only have one nut.|Help I'm trapped in some nerd's computer|I like ice cream|Why don't you take a seat over there?|On a scale of 4-17, you're probably about 11.|I only stick with you because there are no other.|We ride, tonight.|hey its me, ur nigerian prince|What is up, buttercup?|You forget so easy.|Did you know an airbag once saved my life.".Split('|');
        static string user, pass;
        static string authCode;
        static bool isRunning = false;
		static List <String> rLine = new List<String>(tLine);
        static void Main(string[] args)
		{

			//Then create the console, add a title and some text if you want. Include instructions.
			//Set the title
			Console.Title = "iRon";
			Console.WriteLine ("CTRL+C quits the program.");
			//Our first order of buisness is to retrieve a username and password from the bot user so they can log in.
			//Don't forget to add some string variables for user/pass

			//Console.Readline() reads the input from the user.
			user = "packetzero";

			pass = ""; // hohoho nice try
			//We can create a function to make Logging in a bit cleaner, so let's do that.
			SteamLogIn ();
		}
        static void SteamLogIn()
        {
            //Need to add static SteamClient() steamClient. We add this to the top of the project to make it available to entire project.
            //We need to create an instance of the SteamClient() class. 
            steamClient = new SteamClient();

            //We need to add static CallbackManager manager for the same reason.
            //And then create an instance of the CallBackManager class. We put steamClient inside the CallbackManager so we can handle the callbacks.
            manager = new CallbackManager(steamClient);

            //Then, we need to create static SteamUser steamUser;, again, for the same reason.
            //Basically, we set steamUser to the handler of the SteamUser class inside of SteamClient. So it becomes the handler of all Steam User related actions and callbacks.
            steamUser = steamClient.GetHandler<SteamUser>();


            //We need to create the handler for steamfriends, in order to perform
            //Friends related tasks, such as messages and setting our persona state
            steamFriends = steamClient.GetHandler<SteamFriends>();


            //Now, we can't handle any callbacks without registering the Callbacks with the manager, and the fucntion that we use to handle what happens.
            //First, we register the Connection Callback, so that we can perform actions when we have connected to the Steam Network. We also need to create the function that handles 
            new Callback<SteamClient.ConnectedCallback>(OnConnected, manager);

            //We need to create an ondisconnected callback to reconnect when the bot is disconnected
            new Callback<SteamClient.DisconnectedCallback>(OnDisconnected, manager); //See OnDisconnected function

            //We register a JOB callback, a JobCallback is a callback that performs a specific purpose 
            new JobCallback<SteamUser.UpdateMachineAuthCallback>(OnMachineAuth, manager);

			//Now we register the FriendsListCallback, which will fire when the bot recieves its list of friends, (and I think whenever Steam updates it automatically)
			new Callback<SteamFriends.FriendsListCallback>(OnFriendsList, manager);

            //Now we register the Log On callback, and create the OnLoggedOn function
            new Callback<SteamUser.LoggedOnCallback>(OnLoggedOn, manager);

            //Now we register the OnAccountInfo callback, and create its corresponding function
            new Callback<SteamUser.AccountInfoCallback>(OnAccountInfo, manager);

            //Now we register the OnSteamMessage callback, and create its corresponding function
            new Callback<SteamFriends.FriendMsgCallback>(OnChatMessage, manager);

            //Now we create an isRunning bool variable so that we can tell a while loop that we're going to create to run the callback manager as long as isRunning is true.
            isRunning = true;
            //Inform the user we are attempting to connect.
            Console.WriteLine("\nConnecting to Steam...\n");
            //We tell our steamClient class to connect to steam.
            steamClient.Connect();
            //Begin our Callback handling loop. First, we set isRunning to true so that we can begin the loop.
            isRunning = true;
            while (isRunning)
            {
                //Check for callbacks every second.
                manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));

            }
            Console.ReadKey();
        }

        //We give it the ConnectedCallback class, named callback. This is so that we can interact with the callback. Performing specific actions.
        static void OnConnected(SteamClient.ConnectedCallback callback)
        {
            //we check the result of the callback to check if connection was a success or not
            if (callback.Result != EResult.OK)
            {
                //Inform the user of the failure to connect, and the reason why. {0} gets replaced with the callback result
                Console.WriteLine("Unable to connect to steam: {0}\n", callback.Result);
                //Since we didn't connect, we exit the loop.
                isRunning = false;
                //Quit the function.
                return;
            }

            //Now we inform the user that the connection was a success. {0} gets replaced with the username the user entered.
            Console.WriteLine("Connected to Steam. \nLogging in '{0}'...\n", user);
            //We need to create an array of bytes to hold the sentry file's data
            byte[] sentryHash = null;
            //Now we need to check if 'sentry.bin' exists, and if so, to grab the hash of the file and send it to steam later on
            if (File.Exists("sentry.bin"))
            {
                //Now, we load all of the bytes into the array
                byte[] sentryFile = File.ReadAllBytes("sentry.bin");
                //Now, set sentryHash to the SHAHash of the sentry file, which is loaded into the sentryFile byte array
                sentryHash = CryptoHelper.SHAHash(sentryFile);
            }
            //Now we attempt to log on by passing the LogOnDetails to steamUser.LogOn
            steamUser.LogOn(new SteamUser.LogOnDetails
            {
                Username = user,
                Password = pass,
                //We add some new stuff, the authcode we grabbed from the user AND the hash of the sentry file
                AuthCode = authCode,

                SentryFileHash = sentryHash,
            });
            //What we need to do, that is completely necessary, is to create an OnDisconnected callback to reconnect the bot when SteamGuard disconnects it
            //For whatever reason


            //Now we create a Log On callback to tell the bot what to do, like log in.
        }

        //We give it the LoggedOnCallback class, named callback. This is so that we can interact with the callback. Performing specific actions.
        static void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            //Now we check if our log on request was denied
            if (callback.Result == EResult.AccountLogonDenied) //Quick thing, it is supposed to be '==' not '!='
            {
                //Since it was denied, the account must have SteamGuard protection on.
                Console.WriteLine("This account is SteamGuard protected.");
                //We ask the user for the authentication code sent to the email when steam detects an account being accessed from something it doesn't recognize, and inform them of the EMail domain
                //Steam sent the code to
                Console.Write("Please enter the auth code sent to the email at {0}: ", callback.EmailDomain);
                //Now we create a string to hold the authcode
                //Now, what do we do when we need input from the user? Console.ReadLine()
                authCode = Console.ReadLine();
                //Exit the function
                //Create UpdateMachineAuthCallback
                return;
            }
            if (callback.Result != EResult.OK)
            {
                //We inform the user that we couldn't logon to steam, and the reason why.
                Console.WriteLine("Unable to log in to Steam: {0}\n", callback.Result);
                //Since we couldn't log on, we exit the loop.
                isRunning = false;
                //Exit the function
                return;
            }
            //Now we can do things! That is the end of this tutorial, however. Next tutorial we will handle SteamGuard so we can log on into a secure account.
            Console.WriteLine("{0} successfully logged in!", user);
        }

        //We give it the OnMachineAuth class, named callback. This is so that we can interact with the callback. Performing specific actions.
        //First we need to include SystemIO
        //Now, what we need to do is create the function that handles the UpdateMachineAuthCallback
        static void OnMachineAuth(SteamUser.UpdateMachineAuthCallback callback, JobID jobID)
        {
            //inform the user we are updating the sentry file.
            //The sentry file is a special file steam uses to verify whether or not you are authorized.
            Console.WriteLine("Updating sentry file...");
            //What we need to do is grab the hash of the data steam sends back
            //This data is what Steam sends to be put into a sentry file
            byte[] sentryHash = CryptoHelper.SHAHash(callback.Data);
            //Now, we write all the data that Steam sent to us to our new sentry file
            File.WriteAllBytes("sentry.bin", callback.Data);
            //Now we need to inform Steam that we are connecting to a Steam Guard protected account, and have the correct sentry data
            steamUser.SendMachineAuthResponse(new SteamUser.MachineAuthDetails
            {
                JobID = jobID,
                FileName = callback.FileName,
                BytesWritten = callback.BytesToWrite,
                FileSize = callback.Data.Length,
                Offset = callback.Offset,
                Result = EResult.OK,
                LastError = 0,
                OneTimePassword = callback.OneTimePassword,
                SentryFileHash = sentryHash,
            });
            //Inform the user the process is complete
            Console.WriteLine("Done.");
            //Refer to OnConnected steamUser.Logon, adding authcode and sentry hash
        }

        //Now we create an OnDisconnected function to have the bot attempt to reconnect
        static void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            //Inform the user that the bot was disconnected and is now attempting to reconnect
            Console.WriteLine("\n{0} disconnected from Steam, reconnecting in 5...\n", user);
            //Sleep the program for 5 seconds
            Thread.Sleep(TimeSpan.FromSeconds(5));
            //attempt to reconnect
            steamClient.Connect();
        }


        //Now we create our OnAccountInfo function to have the bot perform a specific
        //Action when steam sends it the info of the account it is logged in as
        //What we're going to do is set our "Persona State" to online, this makes the bot
        //Show up as online in people's friends lists
        static void OnAccountInfo(SteamUser.AccountInfoCallback callback)
        {
            //Set our persona state to online
            steamFriends.SetPersonaState(EPersonaState.Online);
            //Now our bot will show up in our friends list
            //Now, let's create a callback for steam messages so that
            //we can respond to them!
        }

        //Creating this callback handler allows us to respond to a message
        //We can perform specific actions based on this message
        //but for now we will just stick with responding to every message
        //with "hello"
        static void OnChatMessage(SteamFriends.FriendMsgCallback callback)
        {
            //This allows us to send a message, callback.sender is the steam ID
            //of the friend who sent the message, therefore we can use it
            //for the SendChatMessage function which requires a steam ID
            //Then, chat entry type is set to chat message in order to
            //inform steam we are sending a text based message
            //and not a "x user is typing..." or something like that

            //this if statement prevents the bot from saqying something every while you're typing
            //because the "x user is typing..." is interperated as a steam message, so
            //we need to check if it's an actual chat message

            //we're going to create a switch statement to mass-check the arguements.


            //before we start off, go to main() and work with the chat.txt
            //we'll need two string variables, one to hold the trimed version of the message, and another to hold the read line.

			Random rnd = new Random();
			int ind = rnd.Next (rLine.Count);
			string rand = rLine [ind];
			Console.WriteLine (steamFriends.GetFriendPersonaName(callback.Sender));
			if (callback.Message.Length > 0) {
				Console.WriteLine (callback.Message);
				Console.WriteLine(rand);
				steamFriends.SendChatMessage (callback.Sender, EChatEntryType.ChatMsg, rand); //if so, respond with the response listed in chat.txt
			}
			if (callback.Message.Length > 5) {
				rLine.Add(callback.Message);
			}
			return; //exit method
        }
		static void OnFriendsList(SteamFriends.FriendsListCallback callback)
		{
			//We need to make the program sleep for a little bit so it has time to cache all the names, if you don't add this part strange things will happen.
			Thread.Sleep(2500);
			Random timing = new Random();
			int nexttime = timing.Next(30,60);
			Random rnd = new Random ();
			Random who = new Random ();
			var friendtimer = new System.Threading.Timer ((e) => {
				List<SteamID> ids = new List<SteamID> ();
				int count = 0;
				nexttime = timing.Next(45, 75);
				//here we will create a foreach loop that will go through our callback.FriendsList and check to see if one of the friends is really someone who wants to be our friend
				//and if so, to add them
				//var is basically a variable declaration for something you don't know will be.
				foreach (var friend in callback.FriendList) {
					//checks our friend relationship to check if they sent a friend request and aren't just a friend already on the list.
					if (friend.Relationship == EFriendRelationship.RequestRecipient) {
						//Adds a friend based on their SteamID, because that is how steam friending works
						steamFriends.AddFriend (friend.SteamID);
						//Sleep half a second, not necessary but is nice.
						Thread.Sleep (500);
						//Notify the user that you are now friends to a bot.
						steamFriends.SendChatMessage (friend.SteamID, EChatEntryType.ChatMsg, "Ahoy friendo!");
					}
					if (friend.Relationship == EFriendRelationship.Friend) {
						ids.Add (friend.SteamID);
						++count;
					}
				}
				string rand = rLine [rnd.Next (rLine.Count)];
  				SteamID newid = ids.ElementAt(who.Next (ids.Count));
				Console.WriteLine(steamFriends.GetFriendPersonaName(newid));
				Console.WriteLine(steamFriends.GetFriendPersonaState(newid));
				Console.WriteLine(DateTime.Now);
				if (steamFriends.GetFriendPersonaState(newid) == EPersonaState.Online || steamFriends.GetFriendPersonaState(newid) == EPersonaState.Away){
					Console.WriteLine (rand);
					steamFriends.SendChatMessage (newid, EChatEntryType.ChatMsg, rand); //if so, respond with the response listed in chat.txt
				}
				}, null, 0, 1*60*1000);
		}
	}
}
