using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;

using Quartz;

namespace Bezviz
{
    public class TwitterJob : IJob
    {
        private const string Yes = "Да.";
        private static readonly Dictionary<string, int> AnswersDict = new Dictionary<string, int> {
                        { "Нет", 2 },
                        { "Еще нет", 2 },
                        { "Все еще нет", 2 },
                        { "Не-а", 2 },
                        { "Немного еще подождать надо", 2 },
                        { "Может в следующем году?", 2 },
                        { "Пока нет", 2 },
                        { "Нет :(", 2 },
                        { "Сами ждем", 2 },
                        { "Да нет же", 2 },
                        { "Nope", 2 },
                        { "Ждите", 2 },
                        { "Скоро уже", 2 },
                        { "К сожалению, нет", 2 },
                        { "Мало ли что вам обещали", 2 },
                        { "НЕТ!", 2 },
                        { "С 1 января точно будет!", 2 },
                        { "🇺🇦 Ні", 1 },
                        { "🇧🇬 Не", 1 },
                        { "🇬🇧 No", 1 },
                        { "🇭🇺 Nincs", 1 },
                        { "🇬🇷 Όχι", 1 },
                        { "🇩🇰 Ingen", 1 },
                        { "🇮🇪 Níl", 1 },
                        { "🇪🇸 No", 1 },
                        { "🇮🇹 No", 1 },
                        { "🇱🇻 Nē", 1 },
                        { "🇱🇹 Ne", 1 },
                        { "🇲🇹 Nru", 1 },
                        { "🇩🇪 Nein", 1 },
                        { "🇳🇱 Nee", 1 },
                        { "🇵🇱 Nie", 1 },
                        { "🇵🇹 Não", 1 },
                        { "🇷🇴 Nu", 1 },
                        { "🇸🇰 Nie", 1 },
                        { "🇸🇮 No", 1 },
                        { "🇫🇮 Ei", 1 },
                        { "🇫🇷 Non", 1 },
                        { "🇭🇷 Ne", 1 },
                        { "🇨🇿 Ne", 1 },
                        { "🇸🇪 Nej", 1 },
                        { "🇪🇪 Ei", 1 }
//            { "Похоже что да, но надо удостовериться...", 1 },
//            { "Давайте немного подождем", 1 },
//            { "Не будем категоричными", 1 },
        };

        private static readonly SingleUserAuthorizer Auth = new SingleUserAuthorizer
        {
            CredentialStore = new SingleUserInMemoryCredentialStore
            {
                ConsumerKey = ConfigurationManager.AppSettings["consumerKey"],
                ConsumerSecret = ConfigurationManager.AppSettings["consumerSecret"],
                AccessToken = ConfigurationManager.AppSettings["accessToken"],
                AccessTokenSecret = ConfigurationManager.AppSettings["accessTokenSecret"]
            }
        };

        private static readonly Random Random = new Random();

        public void Execute(IJobExecutionContext context)
        {
            RunTask();
        }

        public static void RunTask()
        {
            try
            {
                var twitterCtx = new TwitterContext(Auth);
                var answer = Yes;
                if(DateTime.Now < new DateTime(2017, 06, 11))
                {
                    var answers = new List<string>();
                    foreach(var item in AnswersDict)
                    {
                        for(var i = 0; i < item.Value; i++)
                        {
                            answers.Add(item.Key);
                        }
                    }

                    var weekstatuses = twitterCtx.Status.Where(
                                                               s => s.Type == StatusType.User
                                                                   && s.ScreenName == "visafreealready"
                                                                   && s.CreatedAt > DateTime.Now.AddDays(-14)).
                            Select(s => s.Text).
                            Distinct().
                            ToList();

                    int answerNumber;
                    do
                    {
                        answerNumber = Random.Next(answers.Count - 1);
                        Console.WriteLine("answer: {0}", answers[answerNumber]);
                    }
                    while(weekstatuses.Contains(answers[answerNumber]));

                    answer = answers[answerNumber];
                }

                var tweet = twitterCtx.TweetAsync(answer).Result;
                Console.WriteLine(tweet == null ? "an error occured, tweet is null" : string.Format("tweet.StatusID: {0}", tweet.StatusID));
            }
            catch(Exception e)
            {
                Console.WriteLine("an error occured: {0}", e.Message);
            }
        }
    }
}