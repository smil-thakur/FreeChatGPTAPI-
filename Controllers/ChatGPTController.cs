using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using PuppeteerSharp;

namespace Controllers;

[Route("[controller]")]
[ApiController]
public class ChatGPTController : ControllerBase
{
    private static IPage? page = null;
    private static IBrowser? browser;

    public static IBrowser? Browser { get => browser; set => browser = value; }
    public static IPage? Page { get => page; set => page = value; }

    [HttpGet]
    public IActionResult HelloWord()
    {
        return Ok("hello world");
    }

    [HttpGet("initialBrowser")]
    public async Task<IActionResult> InitializeBroswer()
    {
        try
        {
            Browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Browser = SupportedBrowser.Chrome,
                Headless = true,
                ExecutablePath = "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome",
                UserDataDir = "/Users/smil/Library/Application Support/Google/Chrome",
                Args = ["--disable-blink-features=AutomationControlled", "--no-sandbox", "--disable-setuid-sandbox"]
            });

            Page = await Browser.NewPageAsync();
            await Page.SetUserAgentAsync("Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");

            await Page.EvaluateFunctionOnNewDocumentAsync(@"() => {
            Object.defineProperty(navigator, 'webdriver', { get: () => undefined });
            Object.defineProperty(navigator, 'languages', { get: () => ['en-US', 'en'] });
            Object.defineProperty(navigator, 'platform', { get: () => 'MacIntel' });

            const originalQuery = navigator.permissions.query;
            navigator.permissions.query = (parameters) => (
                parameters.name === 'notifications' ?
                    Promise.resolve({ state: 'denied' }) :
                    originalQuery(parameters)
            );
        }");
            return Ok("Webbrowser initialized");
        }
        catch (Exception ex)
        {
            return BadRequest("Could not initialize the browser" + ex.Message);
        }
    }

    [HttpGet("talk")]
    public async Task<IActionResult> StartChatGPTSession(string message)
    {
        if (Page == null || Browser == null)
        {
            return BadRequest("Please initialize browser first!");
        }
        try
        {

            await Page.GoToAsync("https://www.chat.com");

            await Page.WaitForSelectorAsync("#prompt-textarea");

            // Modify the inner HTML of the <p> tag inside #prompt-textarea
            await Page.EvaluateFunctionAsync($@"() => {{
                let textareaDiv = document.getElementById('prompt-textarea');
                if (textareaDiv) {{
                    let pTag = textareaDiv.querySelector('p');
                    if (pTag) {{
                        pTag.innerHTML = 'this is the query = {message}, the result should be one single paragraph, no title no nothing only single paragraph';
                    }}
                }}
            }}");
            // Click the "Send prompt" button
            await Page.ClickAsync("button[aria-label='Send prompt']");
            Thread.Sleep(3000);

            await Page.WaitForFunctionAsync(@"() => {
                return !document.querySelector('.result-streaming');
            }");

            // Get the last element with class "text-message" and extract its nested <p> tag inner HTML
            string lastMessage = await Page.EvaluateFunctionAsync<string>(@"() => {
                let messages = document.querySelectorAll('.text-message');
                if (messages.length > 0) {
                    let lastMessage = messages[messages.length - 1];
                    let pTag = lastMessage.querySelector('p');
                    return pTag ? pTag.innerHTML : 'No message found';
                }
                return 'No messages found';
            }");

            return Ok($"Session started! Last message: {lastMessage}");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("destroyBrowser")]
    public async Task<IActionResult> DestroyBrowser()
    {
        try
        {
            if (page != null && browser != null)
            {
                await page.CloseAsync();
                await browser.CloseAsync();
                return Ok("Broswer Destroyed!");

            }
            else
            {
                return BadRequest("Browser not initialized!");
            }

        }
        catch (Exception ex)
        {
            return BadRequest("Not able to destroy browser" + ex.Message);
        }
    }
}