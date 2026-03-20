
// 1. 先從頁面上抓需要的資料和元素
const roomId = parseInt(document.getElementById("roomId").value);
const currentUserId = parseInt(document.getElementById("currentUserId").value);
const messageList = document.getElementById("messagesList");
const messageInput = document.getElementById("messageInput");
const sendButton = document.getElementById("sendButton");
const messageContainer = document.getElementById("messageContainer");

// 2. 建立 SignalR 連線

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .withAutomaticReconnect()
    .build();

// 3. 監聽：收到新訊息時，加到畫面上

connection.on("ReceiveMessage", function (data) {
    const li = document.createElement("li");
    li.className = "mb-3 border-bottom pb-2";
    if (data.senderId === currentUserId) {
        li.classList.add("text-end");
    }

    li.innerHTML = `
    <div class="fw-bold">${data.senderName}</div>
    <div>${data.message}</div>
    <small class="text-muted">${data.createDate}</small>
    `;

    messageList.appendChild(li);
    messageContainer.scrollTop = messageContainer.scrollHeight; // 滾動到最底部
});

// 4. 啟動連線，成功後加入聊天室

connection.start().then(function () {
    console.log("SignalR 已連線");
    connection.invoke("JoinRoom", roomId);
    messageContainer.scrollTop = messageContainer.scrollHeight;
}).catch(function (err) {
    return console.error("SignalR 連線失敗：", err);
});

// 5. 送出訊息
function sendMessage() {
    const msg = messageInput.value.trim();
    if (msg === "") return;
    connection.invoke("SendMessage", roomId, msg)
        .catch(function (err) {
            console.error("送出訊息失敗：", err);
        });
    messageInput.value = "";
    messageInput.focus();
}

sendButton.addEventListener("click", sendMessage);

messageInput.addEventListener("keyup", function (e) {
    if (e.key === "Enter") {
        sendMessage();
    }
});