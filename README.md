# NamelessBot
基于 [Kaiheila.Net](https://github.com/gehongyan/KaiHeiLa.Net) 构建的 Kook 匿名留言板机器人
## 如何部署
1. 前往 (Releases)[https://github.com/Misaka-L/NamelessBot/releases] 下载最新版本的机器人
2. 编写 `appsettings.json` (见 appsettings.json 格式)
3. 运行 `NamelessBot.Bot.exe` 或 `dotnet NamelessBot.Bot.dll` (Linux) 运行
4. 搞定
## appsettings.json 格式
```json
{
  "Kook": {
    "Token": "这里写机器人的 Token 可以在开发者中心查看"
  },
  "BotMarket": {
    "Enabled": false, // 是否启用 BotMarket 状态监控，保持 false 即可 (毕竟你也不可能拿一个已有公开部署的机器人的源代码去传 BotMarket)
    "BotMarketId": "" // BotMarket 状态监控的 UUID
  }
}
```
