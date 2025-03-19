"use strict";

console.log("Test message");

console.log("sendButton:", document.getElementById("sendButton"));
console.log("messageInput:", document.getElementById("messageInput"));

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub?user_id=" + encodeURIComponent(userId)).build();

// Disable send button until connection is established 
document.getElementById("sendButton").disabled = true;
connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    li.textContent = `${user} says ${message}`;
    document.getElementById("messageslist").appendChild(li);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

*//*document.getElementById("sendButton").addEventListener("click", function (event) {
    console.log("check gui");
const toUserId = document.getElementById("toUserInput").value.trim();
const message = document.getElementById("messageInput").value.trim();
const conversationId = document.getElementById("conversationId").value.trim(); // ID của cuộc trò chuyện
console.log("userId:", userId);
console.log("toUserId:", toUserId);
console.log("message:", message);
console.log("conversationId:", conversationId);

if (!message) {
    alert("Vui lòng điền nội dung tin nhắn");
    return;
}

connection.invoke("SendPrivateMessage", toUserId, message, userId, conversationId).catch(function (err) {
    return console.error(err.toString());
});

const li = document.createElement("li");
li.textContent = `I sent to ${toUserId}: ${message}`;
document.getElementById("messageslist").appendChild(li);

event.preventDefault();
});

document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("sendButton").addEventListener("click", function (event) {
        try {
            console.log("Sending message...");
            const toUserId = document.getElementById("toUserInput").value.trim();
            const message = document.getElementById("messageInput").value.trim();
            const conversationId = document.getElementById("conversationId").value.trim();

            console.log("Kiểm tra kiểu dữ liệu:");
            console.log("toUserId:", toUserId, "-", typeof toUserId);
            console.log("message:", message, "-", typeof message);
            console.log("userId:", userId, "-", typeof userId);
            console.log("conversationId:", conversationId, "-", typeof conversationId);

            if (!message) {
                alert("Vui lòng điền nội dung tin nhắn");
                return;
            }

            // Thực hiện gửi tin nhắn riêng với try-catch để xử lý lỗi
            connection.invoke("SendPrivateMessage", toUserId, message, userId, conversationId).catch(function (err) {
                console.error("Lỗi khi gửi tin nhắn:", err.toString());
                alert("Đã xảy ra lỗi khi gửi tin nhắn. Vui lòng thử lại.");
            });

            const li = document.createElement("li");
            li.textContent = `I sent to ${toUserId}: ${message}`;
            document.getElementById("messageslist").appendChild(li);
        } catch (error) {
            console.error("Lỗi trong sự kiện click gửi tin nhắn:", error);
            alert("Đã xảy ra lỗi không mong muốn. Vui lòng thử lại.");
        }

        event.preventDefault();
    });
});

/*"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/SignalRServer")
    .build();

connection.on("LoadMessages", function () {
    location.href = '/Chat'    //tên Folder
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});*/