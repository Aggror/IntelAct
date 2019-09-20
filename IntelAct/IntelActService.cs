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

            List<string> filterWords = new List<string>() {
                "hi",
                "hallo",
                "hey",
                "hello",
                "google",
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
                "from",
                "with",
                "number",
                "a",
                "an",
                "it",
                "you",
                "our",
                "we",
                "us",
                "of"
            };

            var cleanWords = new List<string>();
            foreach (var inputWord in inputWords)
            {
                if (!filterWords.Contains(inputWord.ToLower()))
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

            var entityKeyWords = new Dictionary<string, string>();
            entityKeyWords.Add("profiel", "profile");
            entityKeyWords.Add("profielen", "profile");
            entityKeyWords.Add("profile", "profile");
            entityKeyWords.Add("profiles", "profile");

            entityKeyWords.Add("factuur", "invoice");
            entityKeyWords.Add("facuren", "invoice");
            entityKeyWords.Add("invoice", "invoice");
            entityKeyWords.Add("invoices", "invoice");

            foreach (var entityKeyWord in entityKeyWords)
            {
                if (cleanedWords.Contains(entityKeyWord.Key))
                {
                    action.Entity = entityKeyWord.Value;
                    cleanedWords.Remove(entityKeyWord.Key);
                    break;
                }
            }


            action.Value = String.Join(",", cleanedWords.ToArray());

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
                new RedirectCrap(){Name = "show", Entity = "profile", Url = "/profiles/", Table = "dbo.profile"},
                new RedirectCrap(){Name = "show", Entity = "profile", Url = "/profiles/show?search=", Table = "dbo.profile"},
                new RedirectCrap(){Name = "edit", Entity = "profile", Url = "/profiles/edit?search=", Table = "dbo.profile"},

                new RedirectCrap(){Name = "show", Entity = "invoice", Url = "/invoice/", Table = "dbo.profile"},
                new RedirectCrap(){Name = "show", Entity = "invoice", Url = "/invoice/show?search=", Table = "dbo.profile"},
                new RedirectCrap(){Name = "edit", Entity = "invoice", Url = "/invoice/edit?search=", Table = "dbo.profile"},
            };
        }
    }
}
