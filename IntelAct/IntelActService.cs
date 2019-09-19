using System;
using System.Collections.Generic;
using System.Linq;

namespace GoogleAssistantWindows.IntelAct
{
    public static class IntelActService
    {

        //1 Clean up the input 
        public static List<string> VeryNoice(string input)
        {
            var inputWords = input.Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            List<string> badWords = new List<string>() {
                "de",
                "het",
                "een",
                "van",
                "door",
                "mij",
                "me",
                "ik",
                "hij",
                "zij",
                "wij",
                "ons",
                "the",
                "a ",
                "an",
                "it",
                "you",
                "our",
                "we",
                "us"
            };

            var cleanWords = new List<string>();
            foreach (var inputWord in inputWords)
            {
                if (!badWords.Contains(inputWord.ToLower()))
                {
                    cleanWords.Add(inputWord.ToLower());
                }
            }

            return cleanWords.ToList();
        }

        //2 Determine actions
        public static ActionStuff DetermineKeyWords(List<string> cleanedWords)
        {
            var actionKeyWords = new Dictionary<string, string>();
            actionKeyWords.Add("show", "show");
            actionKeyWords.Add("zoek", "show");
            actionKeyWords.Add("search", "show");
            actionKeyWords.Add("toon", "show");
            actionKeyWords.Add("tonen", "show");

            actionKeyWords.Add("wijzig", "edit");
            actionKeyWords.Add("bewerk", "edit");
            actionKeyWords.Add("edit", "edit");

            var entities = new List<string>() {
                "profiel",
                "profile",
                "factuur",
                "invoice",
            };

            var action = new ActionStuff();
            foreach (var actionKeyWord in actionKeyWords)
            {
                if (cleanedWords.Contains(actionKeyWord.Key))
                {
                    action.Name = actionKeyWord.Value;
                    cleanedWords.Remove(actionKeyWord.Key);
                    break;
                }
            }

            foreach (var entity in entities)
            {
                if (cleanedWords.Contains(entity))
                {
                    action.Entity = entity;
                    cleanedWords.Remove(entity);
                    break;
                }

            }
            action.Value = cleanedWords.FirstOrDefault();

            return action;
        }

        //3 match action with Redirect
        public static RedirectCrap MatchActionStuffToRedirect(ActionStuff action)
        {
            var exampleRedirectData = SetupExampleData();

            foreach (var redirect in exampleRedirectData)
            {
                if (redirect.Name == action.Name && redirect.Entity == action.Entity)
                {

                    if (String.IsNullOrWhiteSpace(action.Value))
                    {
                        //Geen parameter
                        if (!redirect.Url.Contains("?"))
                        {
                            redirect.action = action;
                            return redirect;
                        }
                    }
                    else
                    {
                        //wel parameter
                        if (redirect.Url.Contains("?"))
                        {
                            redirect.action = action;
                            redirect.Url += action.Value;
                            return redirect;
                        }
                    }
                }
            }

            return null;
        }

        public static List<RedirectCrap> SetupExampleData()
        {
            return new List<RedirectCrap>()
            {
                new RedirectCrap()
                {
                    Name = "show",
                    Entity = "profiel",
                    Url = "/profiles/",
                    Table = "dbo.profile"
                },
                new RedirectCrap()
                {
                    Name = "show",
                    Entity = "profiel",
                    Url = "/profiles/show?search=",
                    Table = "dbo.profile"
                },
                new RedirectCrap()
                {
                    Name = "edit",
                    Entity = "profiel",
                    Url = "/profiles/edit?search=",
                    Table = "dbo.profile"
                }
            };
        }
    }
}
