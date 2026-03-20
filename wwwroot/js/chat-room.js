// ============================
// 1. 先從頁面上抓不受 Vue 影響的資料
// ============================
const roomId = parseInt(document.getElementById("roomId").value);
const currentUserId = parseInt(document.getElementById("currentUserId").value);

// ============================
// 2. 建立 SignalR 連線
// ============================
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .withAutomaticReconnect()
    .build();

// ============================
// 3. 監聽：收到新訊息時，加到畫面上
// ============================
connection.on("ReceiveMessage", function (data) {
    console.log("收到訊息：", data);

    // 每次都重新抓，避免被 Vue 影響
    const messageList = document.getElementById("messageList");
    const messageContainer = document.getElementById("messageContainer");

    if (!messageList) {
        console.error("找不到 messageList 元素");
        return;
    }

    const li = document.createElement("li");

    if (data.senderId === currentUserId) {
        li.className = "msg-right";
    } else {
        li.className = "msg-left";
    }

    li.innerHTML = `
        <div class="msg-bubble">
            <div class="fw-bold small">${data.senderName}</div>
            <div>${data.message}</div>
            <small class="text-muted">${data.createDate}</small>
        </div>
    `;

    messageList.appendChild(li);

    if (messageContainer) {
        messageContainer.scrollTop = messageContainer.scrollHeight;
    }
});

// ============================
// 4. 啟動連線
// ============================
connection.start()
    .then(function () {
        console.log("SignalR 已連線");
        connection.invoke("JoinRoom", roomId);

        var messageContainer = document.getElementById("messageContainer");
        if (messageContainer) {
            messageContainer.scrollTop = messageContainer.scrollHeight;
        }

        // 綁定送出事件（確保 DOM 已準備好）
        var messageInput = document.getElementById("messageInput");
        var sendButton = document.getElementById("sendButton");

        function sendMessage() {
            var msg = messageInput.value.trim();
            if (msg === "") return;

            connection.invoke("SendMessage", roomId, msg)
                .catch(function (err) {
                    console.error("送出失敗：", err);
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
    })
    .catch(function (err) {
        console.error("SignalR 連線失敗：", err);
    });