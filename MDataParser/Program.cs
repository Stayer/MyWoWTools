using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Test
{
    class Program
    {
        static void Main()
        {
            string quest_re = @"\s*\[(?<qid>\d+)\]";
            string progress_re = "\\s*\\[\"progress\"\\]\\s*=\\s*\"(?<prog>[^\"]*)\",";
            string complete_re = "\\s*\\[\"complete\"\\]\\s*=\\s*\"(?<comp>[^\"]*)\",";
            string data_re = "\\s*\\[\"chardata\"\\]\\s*=\\s*\"(?<data>[^\"]*)\",";
            string full_re = quest_re + "\\s*=\\s*\\{" + progress_re + complete_re + data_re + "\\s*\\},";
            string full_re2 = quest_re + "\\s*=\\s*\\{" + progress_re + data_re + complete_re + "\\s*\\},";
            string try_s = string.Empty;

            // if (Environment.GetCommandLineArgs()[1].Length>0) Console.WriteLine("ВОТ ОНО ВОТ ОНО {0}", Environment.GetCommandLineArgs()[1]);
            if (!File.Exists("MDataParser.lua"))
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Файл MDataParser.lua не найден! Поместите его в директорию программы!");
                Console.ReadKey();
            }

            using (StreamReader input = new StreamReader("MDataParser.lua"))
            {
                try_s = input.ReadToEnd();
                try_s = Regex.Replace(try_s, "'", "\\'");
            }
            Regex regex = new Regex(full_re);
            MatchCollection matches = regex.Matches(try_s);
            using (StreamWriter output = new StreamWriter("queries.sql", true))
            {
                Console.WriteLine("Добро пожаловать в парсер окончаний квестов! Введите 1, если вы хотите сделать UPDATE в quest_template, и 0 - если хотите в locales_quest. После ввода нажмите Enter");
                int variant = Console.Read();
                if (variant == 49)
                {
                    foreach (Match match in matches)
                    {
                        GroupCollection groups = match.Groups;
                        if (groups["prog"].Length > 0)
                            output.WriteLine("UPDATE `quest_template` SET `RequestItemsText` = '{0}' WHERE `Id` = {1};", groups["prog"], groups["qid"]);
                        if (groups["comp"].Length > 0)
                            output.WriteLine("UPDATE `quest_template` SET `OfferRewardText` = '{0}' WHERE `Id` = {1};", groups["comp"], groups["qid"]);

                        output.WriteLine("-- This data received FROM : {0}", groups["data"]);
                        output.WriteLine();
                    }

                    Regex re2 = new Regex(full_re2);
                    MatchCollection m2 = re2.Matches(try_s);
                    foreach (Match match in m2)
                    {
                        GroupCollection groups = match.Groups;
                        if (groups["prog"].Length > 0)
                            output.WriteLine("UPDATE `quest_template` SET `RequestItemsText` = '{0}' WHERE `Id` = {1};", groups["prog"], groups["qid"]);
                        if (groups["comp"].Length > 0)
                            output.WriteLine("UPDATE `quest_template` SET `OfferRewardText` = '{0}' WHERE `Id` = {1};", groups["comp"], groups["qid"]);
                        output.WriteLine("-- This data received FROM : {0}", groups["data"]);
                        output.WriteLine();
                    }
                }
                else
                {
                    foreach (Match match in matches)
                    {
                        GroupCollection groups = match.Groups;
                        if (groups["prog"].Length > 0)
                            output.WriteLine("UPDATE `locales_quest` SET `RequestItemsText_loc8` = '{0}' WHERE `entry` = {1};", groups["prog"], groups["qid"]);
                        if (groups["comp"].Length > 0)
                            output.WriteLine("UPDATE `locales_quest` SET `OfferRewardText_loc8` = '{0}' WHERE `entry` = {1};", groups["comp"], groups["qid"]);
                        output.WriteLine("-- This data received FROM : {0}", groups["data"]);
                        output.WriteLine();
                    }

                    Regex re2 = new Regex(full_re2);
                    MatchCollection m2 = re2.Matches(try_s);
                    foreach (Match match in m2)
                    {
                        GroupCollection groups = match.Groups;
                        if (groups["prog"].Length > 0)
                            output.WriteLine("UPDATE `locales_quest` SET `RequestItemsText_loc8` = '{0}' WHERE `entry` = {1};", groups["prog"], groups["qid"]);
                        if (groups["comp"].Length > 0)
                            output.WriteLine("UPDATE `locales_quest` SET `OfferRewardText_loc8` = '{0}' WHERE `entry` = {1};", groups["comp"], groups["qid"]);
                        output.WriteLine("-- This data received FROM : {0}", groups["data"]);
                        output.WriteLine();
                    }
                }
            }
            Console.WriteLine("Успех! Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }
}