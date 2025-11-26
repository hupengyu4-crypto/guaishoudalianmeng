const unityNamespace = {
  DATA_CDN: 'https://www.alipay.com',
  // 自定义bundle中的hash长度
  bundleHashLength: 32,
  // 单位Bytes, 1MB = 1024 KB = 1024*1024Bytes
  releaseMemorySize: 31457280,
  // 当前appid扩容后，通过本字段告知插件本地存储最大容量，单位Bytes
  maxStorage: 209715200,
  //Unity编辑器版本
  unityVersion: '',
  UnityLoader: {
    UnityCache: {},
  },
};
// 判断是否需要自动缓存的文件，返回true自动缓存；false不自动缓存
unityNamespace.isCacheableFile = function (path) {
  // 判定为下载bundle的路径标识符，此路径下的下载，会自动缓存
  const cacheableFileIdentifier = ['StreamingAssets'];
  // 命中路径标识符的情况下，并不是所有文件都有必要缓存，过滤下不需要缓存的文件
  const excludeFileIdentifier = ['json'];
  if (
    cacheableFileIdentifier.some(
      (identifier) =>
        path.includes(identifier) &&
        excludeFileIdentifier.every(
          (excludeIdentifier) => !path.includes(excludeIdentifier)
        )
    )
  ) {
    return true;
  }
  return false;
};
// 清理缓存时是否可被自动清理；返回true可自动清理；返回false不可自动清理
unityNamespace.isErasableFile = function (info) {
  // 达到缓存上限时，不会被自动清理的文件
  const inErasableIdentifier = [];
  if (
    inErasableIdentifier.some((identifier) => info.path.includes(identifier))
  ) {
    return false;
  }
  return true;
};
window.unityNamespace = unityNamespace;
export default unityNamespace;
