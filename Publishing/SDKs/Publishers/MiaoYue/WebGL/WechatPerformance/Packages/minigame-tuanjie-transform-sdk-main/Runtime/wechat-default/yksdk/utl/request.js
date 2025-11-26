export const GET = 'GET'
export const POST = 'POST'


export function request(method, url, json, success, fail) {
    return new Promise((resolve, reject) => {
        const header = {
            'content-type': "application/json"
        }
        wx.request({
            url: url,
            method: method,
            data: JSON.stringify(json),
            dataType: "json",
            header: header,
            success(res) {
                console.log("statusCode: "+res.statusCode)
                if (res.statusCode === 200) {
                    if (res.data) {
                        if (typeof success === 'function') {
                            success(res.data)
                        }
                        resolve(res.data)
                        return;
                    }
                }

                console.error('Network Error!')
                if (typeof fail === 'function') {
                    fail('Network Error!')
                }
                reject('Network Error!')

            },
            fail(res) {
                console.error(res)
                if (typeof fail === 'function') {
                    fail(res.errMsg)
                }
                reject(res.errMsg)
            }
        })
    })
}


export function requestForm(url, form, success, fail) {
    return new Promise((resolve, reject) => {
        const header = {
            'content-type': "application/x-www-form-urlencoded" //使用form方式传递参数
        }
        wx.request({
            url: url,
            method: POST,
            data: Util.json2Form(form),
            dataType: "json",
            header: header,
            success(res) {
                if (res.statusCode === 200) {
                    if (res.data.state) {
                        if (typeof success === 'function') {
                            success(res.data)
                        }
                        resolve(res.data)
                        return;
                    }
                }

                console.error('Network Error!')
                if (typeof fail === 'function') {
                    fail('Network Error!')
                }
                reject('Network Error!')

            },
            fail(res) {
                console.error(res)
                if (typeof fail === 'function') {
                    fail(res.errMsg)
                }
                reject(res.errMsg)
            }
        })
    })
}


export function post(url, json, success, fail) {
    return request(POST, url, json, success, fail)
}
export function get(url, success, fail) {
    return request(GET, url, null, success, fail)
}

function json2Form(json) {
    var str = [];
    for (var p in json) {
        str.push(encodeURIComponent(p) + "=" + encodeURIComponent(json[p]));
    }
    return str.join("&");
}