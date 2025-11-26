export const GET = 'GET'
export const POST = 'POST'


export function request(method, url, jsonstring, success, fail) {
    return new Promise((resolve, reject) => {
        const header = {
            'content-type': "application/x-www-form-urlencoded" //使用form方式传递参数
        }
        wx.request({
            url: url,
            method: method,
            data: Util.json2Form(jsonstring),
            dataType: "json",
            header: header,
            success(res) {
                if (res.statusCode === 200) {
                    if (res.data.state) {
                        success(res.data)
                    } else {

                        fail('request interface error')
                    }

                } else {
                    console.error('Network Error!')
                    fail('Network Error!')
                }
            }
        })
    })
}



function json2Form(json) {
    var str = [];
    for (var p in json) {
        str.push(encodeURIComponent(p) + "=" + encodeURIComponent(json[p]));
    }
    return str.join("&");
}