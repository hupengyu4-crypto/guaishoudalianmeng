mergeInto(LibraryManager.library, {
    YKSDK_Call_JS: function (namePtr, argsPtr, cid) {
		var name = UTF8ToString(namePtr);
        var args = UTF8ToString(argsPtr);
        var str = GameGlobal.YKSDK_call(name, args, cid);
		if (str === null || typeof str == "undefined") {
			str = "";
		}

		var size = lengthBytesUTF8(str + "") + 1;
		var buff = _malloc(size);
		stringToUTF8(str, buff, size);
		return buff;
    }
});
