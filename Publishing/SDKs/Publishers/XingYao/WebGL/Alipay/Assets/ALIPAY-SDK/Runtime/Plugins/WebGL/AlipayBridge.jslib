mergeInto(LibraryManager.library, {
  unityCallJs: function (eventId, paramJson) {
    if (typeof UnityJsBridge === 'object') {
      UnityJsBridge.handleMsgFromUnity(
        UTF8ToString(eventId),
        UTF8ToString(paramJson)
      );
    } else if (
      window.webkit &&
      typeof window.webkit.messageHandlers !== 'undefined'
    ) {
      if (
        typeof window.webkit.messageHandlers.handleMsgFromUnity === 'object'
      ) {
        window.webkit.messageHandlers.handleMsgFromUnity.postMessage({
          apiName: UTF8ToString(eventId),
          apiParam: UTF8ToString(paramJson),
        });
      } else {
        console.error('unity send message failed');
      }
    } else {
      console.error('platform not support');
    }
  },
  unityCallJsSync: function (eventId, paramJson) {
    var res = UnityJsBridge.handleMsgFromUnitySync(
      UTF8ToString(eventId),
      UTF8ToString(paramJson)
    );
    var bufferSize = lengthBytesUTF8(res || '') + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(res, buffer, bufferSize);
    return buffer;
  },
  _free: function (ptr) {},
  GetAlipayEnv: function () {
    var res = my.env;
    var jsonString = JSON.stringify(res);
    var bufferSize = lengthBytesUTF8(jsonString || '') + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(jsonString, buffer, bufferSize);
    return buffer;
  },
  AlipayWriteFileSync: function (fileNamePtr, dataPtr, encodingPtr) {
    var fs = my.getFileSystemManager();
    var fileName = UTF8ToString(fileNamePtr);
    var encoding = UTF8ToString(encodingPtr);
    var stringData = UTF8ToString(dataPtr);

    var res = fs.writeFileSync({
      filePath: fileName,
      data: stringData,
      encoding: encoding,
    });
    var jsonString = JSON.stringify(res);
    var bufferSize = lengthBytesUTF8(jsonString || '') + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(jsonString, buffer, bufferSize);
    return buffer;
  },
  AlipayReadFileSync: function (fileNamePtr, encodingPtr) {
    var fs = my.getFileSystemManager();
    var fileName = UTF8ToString(fileNamePtr);
    var encoding = UTF8ToString(encodingPtr);

    var res = fs.readFileSync({
      filePath: fileName,
      encoding: encoding,
    });

    if (res && res.data) {
      var jsonString = JSON.stringify(res);
      var bufferSize = lengthBytesUTF8(jsonString || '') + 1;
      var buffer = _malloc(bufferSize);
      stringToUTF8(jsonString, buffer, bufferSize);
      return buffer;
    }
  },

  AlipayWriteBinFileSync: function (fileNamePtr, dataPtr, dataLength) {
    var fs = my.getFileSystemManager();
    var fileName = UTF8ToString(fileNamePtr);
    var data = HEAPU8.subarray(dataPtr, dataPtr + dataLength);
    var arrayBuffer = data.buffer.slice(
      data.byteOffset,
      data.byteOffset + dataLength
    );
    var res = fs.writeFileSync({
      filePath: fileName,
      data: arrayBuffer,
    });
    var jsonString = JSON.stringify(res);
    var bufferSize = lengthBytesUTF8(jsonString || '') + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(jsonString, buffer, bufferSize);
    return buffer;
  },

  AlipayWriteBinFile: function (
    fileNamePtr,
    dataPtr,
    dataLength,
    callbackIDPtr
  ) {
    var fs = my.getFileSystemManager();
    var fileName = UTF8ToString(fileNamePtr);
    var data = HEAPU8.subarray(dataPtr, dataPtr + dataLength);
    var callbackID = UTF8ToString(callbackIDPtr);

    var arrayBuffer = data.buffer.slice(
      data.byteOffset,
      data.byteOffset + dataLength
    );
    fs.writeFile({
      filePath: fileName,
      data: arrayBuffer,
      success: function (res) {
        var resultString = JSON.stringify({
          messageId: callbackID,
          result: res,
        });
        SendMessage('AlipayBridge', 'ReceiveMessageFromJS', resultString);
      },
      fail: function (err) {
        var resultString = JSON.stringify({
          messageId: callbackID,
          result: err,
        });
        SendMessage('AlipayBridge', 'ReceiveMessageFromJS', resultString);
      },
    });
  },

  AlipayReadBinFileSync: function (fileNamePtr) {
    var fs = my.getFileSystemManager();
    var fileName = UTF8ToString(fileNamePtr);
    var res = fs.readFileSync({
      filePath: fileName,
    });
    if (res && res.data) {
      var byteArray = new Uint8Array(res.data);
      var length = byteArray.length;
      var bufferPtr = _malloc(length + 4);
      HEAP32[bufferPtr >> 2] = length;
      HEAPU8.set(byteArray, bufferPtr + 4);
      return bufferPtr;
    } else {
      var jsonString = JSON.stringify(res);
      var errorBufferSize = lengthBytesUTF8(jsonString) + 1;
      var errorBufferPtr = _malloc(errorBufferSize);
      stringToUTF8(jsonString, errorBufferPtr, errorBufferSize);
      return errorBufferPtr;
    }
  },

  AlipayReadBinFile: function (fileNamePtr, callbackIDPtr) {
    var fs = my.getFileSystemManager();
    var fileName = UTF8ToString(fileNamePtr);
    var callbackID = UTF8ToString(callbackIDPtr);
    var resultString;
    fs.readFile({
      filePath: fileName,
      success: function (res) {
        if (res && res.data) {
          var byteArray = new Uint8Array(res.data);
          resultString = JSON.stringify({
            messageId: callbackID,
            result: byteArray,
          });
        } else {
          resultString = JSON.stringify({
            messageId: callbackID,
            result: null,
          });
        }
        SendMessage('AlipayBridge', 'ReceiveMessageFromJS', resultString);
      },
      fail: function (err) {
        resultString = JSON.stringify({
          messageId: callbackID,
          result: err,
        });
        SendMessage('AlipayBridge', 'ReceiveMessageFromJS', resultString);
      },
    });
  },

  GetFSStatsSync: function (path, recursive) {
    var fs = my.getFileSystemManager();
    var pathStr = UTF8ToString(path);
    var res = fs.statSync({
      path: pathStr,
      recursive: recursive,
    });
    var jsonString = JSON.stringify(res);
    var bufferSize = lengthBytesUTF8(jsonString || '') + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(jsonString, buffer, bufferSize);
    return buffer;
  },

  StatsIsDirectory: function (pathPtr, recursive) {
    var path = UTF8ToString(pathPtr);
    var fs = my.getFileSystemManager();
    try {
      var res = fs.statSync({
        path: path,
        recursive: recursive,
      });

      return res.stats.isDirectory() ? 1 : 0;
    } catch (err) {
      console.log('StatsIsDirectory error: ' + err);
      return 0;
    }
  },

  StatsIsFile: function (pathPtr, recursive) {
    var path = UTF8ToString(pathPtr);
    var fs = my.getFileSystemManager();
    try {
      var res = fs.statSync({
        path: path,
        recursive: recursive,
      });

      return res.stats.isFile() ? 1 : 0;
    } catch (err) {
      console.log('StatsIsFile error: ' + err);
      return 0;
    }
  },
});
