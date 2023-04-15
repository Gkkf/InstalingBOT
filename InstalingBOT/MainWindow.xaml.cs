using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V111.IndexedDB;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;

namespace InstalingBOT
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Start_session(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            button.IsEnabled = false;

           
                Words.InsertWords();
                
                //Console.WriteLine("Ropoczynam rozwiązywanie Instaling...");
                EdgeOptions options = new EdgeOptions();
                //options.AddArgument("--headless");
                //options.EnableMobileEmulation("iPhone X");
                options.AddAdditionalOption("userAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3 Edge/16.16299");
                IWebDriver webDriver = new EdgeDriver(options);
                webDriver.Navigate().GoToUrl("https://instaling.pl/teacher.php?page=login");

                IWebElement loginInput = webDriver.FindElement(By.Id("log_email"));
                loginInput.SendKeys(UserConfig.login);
                IWebElement passInput = webDriver.FindElement(By.Id("log_password"));
                passInput.SendKeys(UserConfig.password);

                Actions actions = new Actions(webDriver);
                actions.SendKeys(Keys.Enter).Perform();

                string[] linkElement = webDriver.Url.Split('=');
                string userId = linkElement[linkElement.Length - 1];

               // Console.WriteLine("ID użytkownika: " + userId);

                webDriver.Navigate().GoToUrl("https://instaling.pl/ling2/html_app/app.php?child_id=" + userId);

                IWebElement checkIssetContinue = webDriver.FindElement(By.Id("continue_session_page"));
                IWebElement checkStart = webDriver.FindElement(By.Id("start_session_page"));
                if (checkIssetContinue.GetCssValue("display") != "none")
                {
                    //Console.WriteLine("Wykryto dokończenie sesji...");
                    IWebElement continueSessionButton = webDriver.FindElement(By.Id("continue_session_button"));
                    continueSessionButton.Click();
                }
                else if (checkStart.GetCssValue("display") != "none")
                {
                   // Console.WriteLine("Wykryto brak sesji...");
                    IWebElement startSessionButton = webDriver.FindElement(By.Id("start_session_button"));
                    startSessionButton.Click();
                }

                while (true)
                {
                    Thread.Sleep(500);
                IWebElement translateCharacters = null;
                IWebElement inputDiv = null;
                IWebElement nextButton = null;
                IWebElement nextwordButton = null;
                IWebElement trans = null;
                IWebElement green = null;
                IWebElement logout = null;
                    IWebElement returnButton = webDriver.FindElement(By.Id("return_mainpage"));
                if(returnButton.Displayed)
                {
                    actions.SendKeys(Keys.Enter).Perform();
                    try
                    {
                        logout = webDriver.FindElement(By.LinkText("Wyloguj"));

                        if (logout.Displayed)
                        {
                            logout.Click();
                            Words.GetNewWords();
                            break;
                        }

                    }
                    catch { }
                    webDriver.Quit();
                    this.Close();
                }else
                {
                    translateCharacters = webDriver.FindElements(By.ClassName("translations"))[0];
                    inputDiv = webDriver.FindElement(By.Id("answer"));
                    nextButton = webDriver.FindElement(By.Id("check"));
                    nextwordButton = webDriver.FindElement(By.Id("nextword"));
                    trans = webDriver.FindElement(By.Id("word"));
                }


                
                    
                  
                    string transNiemickie = trans.Text;
                    bool check = true;
                    string translateCharactersVal = translateCharacters.Text;
                    Word sessionWord = Words.sessionWords.Find(x => x.polish == translateCharactersVal);
                if (sessionWord != null)
                {
                    try
                    {
                        logout = webDriver.FindElement(By.LinkText("Wyloguj"));

                        if (logout.Displayed)
                        {
                            logout.Click();
                            Words.GetNewWords();
                            break;
                        }
                        
                    }
                    catch { }
                    
                    if (inputDiv.Displayed)
                    {
                        inputDiv.SendKeys(sessionWord.german);
                        Thread.Sleep(500);
                        actions.SendKeys(Keys.Enter).Perform();
                    } else {
                        if (nextButton.Displayed)
                        {
                            Thread.Sleep(500);
                            nextButton.Click();
                        }
                        else if (nextwordButton.Displayed) 
                        {
                            Thread.Sleep(500);
                            nextwordButton.Click();
                        }
                        else
                        {
                            actions.SendKeys(Keys.Enter).Perform();
                        }

                    }
                    continue;
                }
                if (Words.translateActive.Contains(translateCharactersVal) == false)
                {
                    if (nextButton.Displayed)
                    {
                        Thread.Sleep(500);

                        nextButton.Click();

                        IWebElement pl = webDriver.FindElement(By.Id("answer_translations"));
                        IWebElement de = webDriver.FindElement(By.Id("word"));

                        Word word = new Word { german = de.Text, polish = pl.Text };
                        Words.sessionWords.Add(word);


                        try
                        {
                            green = webDriver.FindElement(By.Id("green"));

                            if (green.Text != "")
                            {
                                check = false;
                            }
                        }
                        catch { }
                    }

                    if (check == false)
                    {
                        Thread.Sleep(500);

                        nextwordButton.Click();
                    }

                    Thread.Sleep(500);
                    trans = webDriver.FindElement(By.Id("word"));

                    Words.wordsList.Add(new Word { polish = translateCharactersVal, german = trans.Text });
                    Words.translateActive.Add(translateCharactersVal);
                    Thread.Sleep(500);

                    actions.SendKeys(Keys.Enter).Perform();
                    continue;
                }
                else
                {
                    Thread.Sleep(500);

                    Word valuefromdatabase = Words.wordsList.Find(x => x.polish == translateCharactersVal);
                    IWebElement pl = webDriver.FindElement(By.Id("answer_translations"));
                    IWebElement de = webDriver.FindElement(By.Id("word"));

                    Word word = new Word { german = de.Text, polish = pl.Text };
                    Words.sessionWords.Add(word);


                    if (inputDiv.Displayed)
                    {
                        inputDiv.SendKeys(valuefromdatabase.german);
                        Thread.Sleep(500);

                        actions.SendKeys(Keys.Enter).Perform();
                    }
                    else
                    {
                        Thread.Sleep(500);

                        actions.SendKeys(Keys.Enter).Perform();
                    }

                    try
                    {
                        logout = webDriver.FindElement(By.LinkText("Wyloguj"));

                        if (logout.Displayed)
                        {
                            logout.Click();
                            Words.GetNewWords();
                            break;
                        }
                    }
                    catch { }
                    continue;

                }
                webDriver.Quit();
                //Console.WriteLine(translateCharactersVal);
            }
            this.Close();  
        }
    }
}
