Firebaseのディレクトリ構成

user(コレクション)
  |-UserId(ドキュメント) 
    |- name: string
    |- email: string

ChatRooms(コレクション)
  |-RoomId(ドキュメント)
    |-members: List<string>
    |-messages(サブコレクション)
      |-messageId1:{ senderId: "userId_A", message: "Hello", timestamp: ... }
      |-messageId2:{ senderId: "userId_B", message: "World", timestamp: ... }
