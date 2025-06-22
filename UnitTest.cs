using NUnit.Framework.Legacy;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Practice;

public class Tests
{
    public IWebDriver driver;
    public WebDriverWait wait;



    [SetUp]
    public void Setup()
    {
        driver = new  ChromeDriver();
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        wait = new WebDriverWait(driver,TimeSpan.FromSeconds(3));
    }
    
    [TearDown]
    public void TearDown()
    {
        driver.Quit();
        driver.Dispose();
    }
    private void Authorize()
    {
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/");
    
        var login = driver.FindElement(By.Id("Username"));
        login.SendKeys("Test@mail.ru");
        
        var password = driver.FindElement(By.Id("Password"));
        password.SendKeys("test");
        
        var enter = driver.FindElement(By.Name("button"));
        enter.Click();

        wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[data-tid='Title']")));
    }

    private void BigScreen_Chrome()
    {
        //"Костыль" для одного теста
        driver.Quit();
        var options = new ChromeOptions();
        options.AddArguments("--start-maximized");
        driver = new ChromeDriver(options);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        wait = new WebDriverWait(driver,TimeSpan.FromSeconds(3));
    }
    [Test]
    public void Authorization_Test()
    {
        Authorize();
        Assert.That(driver.Title, Does.Contain("Новости"),"На главной странице,после авторизации, не найден заголовок Новости");  
    }

    [Test]
    public void Navigation_MyProfile_SmallScreen()
    {
        Authorize();
        
        var SidebarMenuButton = driver.FindElement(By.CssSelector("[data-tid='SidebarMenuButton']"));
        SidebarMenuButton.Click();
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='SidePage__root']")));

        var avatar = driver.FindElement(By.CssSelector("[data-tid='SidePageBody']"))
                           .FindElement(By.CssSelector("[data-tid='Avatar']"));
        avatar.Click();
        
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='ContactCard']")));

        Assert.That(driver.Title, Does.Contain("Профиль"),"При переходе на страницу не был найден заголовок Профиль");
    }

 [Test]
    public void Navigation_MyProfile_BigScreen()
    {
        
        BigScreen_Chrome();
        Authorize();
        
        var avatar = driver.FindElement(By.CssSelector("[data-tid='DropdownButton']"));
        avatar.Click();
        
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='PopupContent']")));

        var profile = driver.FindElement(By.CssSelector("[data-tid='Profile']"));
        profile.Click();
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='ContactCard']")));
        
        Assert.That(driver.Title, Does.Contain("Профиль"),"При переходе на страницу не был найден заголовок Профиль");
    }


    [Test]
    public void Edit_AdditionalEmail_MyProfile()
    {
        Authorize();

        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/profile/settings/edit");
        wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[data-tid='Title']")));

        var additional = driver.FindElement(By.CssSelector("[data-tid='AdditionalEmail']"))
                                    .FindElement(By.CssSelector("input[type='text']"));
        additional.Click();
        additional.SendKeys(Keys.Control + "a"); 
        additional.SendKeys(Keys.Delete);
        additional.SendKeys("tester@mail.ru");

        driver.FindElement(By.TagName("body")).SendKeys(Keys.Home);
        
        var save = driver.FindElement(By.CssSelector("[data-tid='PageHeader']"))
                         .FindElement(By.XPath("//button[contains(text(), 'Сохранить')]"));
        save.Click();

        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='ContactCard']")));
        
        var email = driver.FindElement(By.CssSelector("[data-tid='ContactCard']"));
        Assert.That(email.Text, Does.Contain("tester@mail.ru"),"Не найден введенный дополнительный адрес эл.почты");
    }
    
    [Test]
    public void Join_OpenCommunity()
    {
        Authorize();

        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/communities/9df6a619-d106-48a6-9ed2-6ace3a317470");
        wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[data-tid='PageHeader']")));

        var join = driver.FindElement(By.CssSelector("[data-tid='Join']"));
        join.Click();

        wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[data-tid='AddButton']")));
        
        var write = driver.FindElement(By.CssSelector("[data-tid='AddButton']"));
        Assert.That(write.Displayed,"Не удалось вступить в сообщество");
        
    }

    [Test]
    public void Search_File()
    {
        Authorize();

        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/files");
        wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[data-tid='Title']")));

        var search = driver.FindElement(By.CssSelector("[data-tid='Search']"));
        search.Click();

        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='ModalPageHeader']")));
        
        var searchInput = driver.FindElement(By.CssSelector("[placeholder='Введите название файла или папки']"));
        searchInput.SendKeys("тест");

        var result = driver.FindElement(By.CssSelector("[data-tid='ModalPageBody']"))
                           .FindElement(By.CssSelector("[data-tid='ListItemWrapper']"));
       
        Assert.That(result.Text, Does.Match(@".*(?i)тест.*"),"В результатах поиска нет совпадения");
        
  }
}
