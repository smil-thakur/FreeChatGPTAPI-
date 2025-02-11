# Free ChatGPT API (Unofficial) 🚀  

A **free and unofficial ChatGPT API** built using **automation and web scraping** with Puppeteer. This API allows you to interact with ChatGPT **without needing OpenAI's API key**!  

---

## 🌟 **Features**
✅ No OpenAI API key required  
✅ Automates ChatGPT interactions using **Puppeteer/Playwright**  
✅ Supports message streaming and real-time responses  
✅ **Session-based workflow**: Initialize → Talk → Destroy  
✅ Customizable **response format** (default is a simple paragraph)  
✅ Lightweight and **easy to deploy**  

---

## ⚡ **How It Works**
This API automates a web browser to:  
1️⃣ **Initialize the browser** using `/api/browser/initialBrowser`  
2️⃣ **Send prompts & receive responses** using `/api/chat/talk`  
3️⃣ **Destroy the browser instance** using `/api/browser/destroyBrowser`  

It **waits for message streaming to complete** before returning a response.  

---

## 🛠 **Installation**
### **1️⃣ Clone the Repository**
```sh
git clone https://github.com/smil-thakur/FreeChatGPTAPI-.gtp
cd FreeChatGPTAPI-
