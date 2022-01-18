function onFileUpload(e) {
    if (!window.FileReader) {
        alert("该浏览器不支持HTML5，请升级或者更换其它浏览器。");
        return;
    }
    var fileInfo = e.currentTarget.files[0];
    var fileName = fileInfo.name;
    var fileSize = fileInfo.size;
    var isImg = fileInfo.type.startsWith("image/");

    var fileReader = new FileReader();
    //var blob = fileInfo.slice(0, fileSize);
    //fileReader.readAsArrayBuffer(blob);
    fileReader.readAsArrayBuffer(fileInfo);
    fileReader.onload = function (result) {
        var pakoString = getUploadingFileContentString(this.result);
        $.ajax({
            url: "http://localhost/Server/ashx/FileProcess.ashx",
            type: "POST",
            data: {
                uploadType: isImg ? "imgUpload" : "fileUpload",
                fileContent: pakoString,
                fileName: fileName,
                isUploadPartly: "false"
            },
            //dataType: "",
            success: function (data) {
                if (isImg) {
                    $("#selectedImage").attr("src", data);
                }
                else {
                    if (data == "True") {
                        alert("上传成功！");
                    }
                    else {
                        alert("上传失败");
                    }
                }
            },
            error: function (e) {
                alert(e.responseText);
            }
        });
    }

    /*
        数据用FormData传递
        后台用System.Web.HttpContext.Current.Request.Files[0]接收HttpPostedFile信息

        fileReader.readAsArrayBuffer(fileInfo);
		fileReader.onload = function (e) {
			var blob = new Blob([this.result]);
			//构造form数据
			var fd = new FormData();
			fd.append('file', blob);
			fd.append('FileName', fileName);

			var xhr = null;
			if (window.XMLHttpRequest) {
				xhr = new XMLHttpRequest();
			}
			else if(window.ActiveXObject) {
				xhr = new ActiveXObject("Microsoft.XMLHTTP");
			}
			if (xhr == null) {
				return false;
			}
			xhr.open('post', 'UploadFile2', true);
			xhr.onreadystatechange = function () {
				if (xhr.readyState == 4 && xhr.status == 200) {
					alert(图片保存成功!");
				}
			}
			//开始发送
			xhr.send(fd);
		}
    */
}

function onLargeFileUpload(e) {
    if (!window.FileReader) {
        alert("该浏览器不支持HTML5，请升级或者更换其它浏览器。");
        return;
    }
    var fileInfo = e.currentTarget.files[0];
    var fileName = fileInfo.name;
    var fileSize = fileInfo.size;
    var currentUplaodCount = 0; // 记录文件已经上传了所少KB

    var fileReader = new FileReader();
    readFilePartly(fileInfo, 0, fileReader);
    fileReader.onload = function (result) {
        currentUplaodCount += result.loaded;
        var pakoString = getUploadingFileContentString(this.result);

        $.ajax({
            url: "http://localhost/Server/ashx/FileProcess.ashx",
            type: "POST",
            data: {
                uploadType: "fileUpload",
                fileContent: pakoString,
                fileName: fileName,
                isUploadPartly: "true"
            },
            //dataType: "",
            success: function (data) {
                if (data == "True") {
                    if (currentUplaodCount >= fileSize) {
                        alert("上传成功！");
                    }
                    else {
                        readFilePartly(fileInfo, currentUplaodCount, fileReader);
                    }
                }
                else {
                    alert("上传失败");
                }
            },
            error: function (e) {
                alert(e.responseText);
            }
        });
    }
}

function getUploadingFileContentString(readResult) {
    if (readResult == null) {
        return null;
    }
    var fileContentArr = new Uint8Array(readResult);
    var fileContentStr = "";
    for (var i = 0; i < fileContentArr.length; i++) {
        fileContentStr += String.fromCharCode(fileContentArr[i]);
    }
    //如果不压缩，直接转base64 string进行传输
    //window.btoa: 将ascii字符串或二进制数据转换成一个base64编码过的字符串,该方法不能直接作用于Unicode字符串.
    //var base64FileString = window.btoa(fileContentStr);
    //return base64FileString;

    //压缩
    var pakoBytes = pako.gzip(fileContentStr);
    var pakoString = fromByteArray(pakoBytes);
    return pakoString;
}

function readFilePartly(fileContent, startIndex, fileReader) {
    var blob = fileContent.slice(startIndex, startIndex + 1048576); // 1024 * 1024 = 1048576
    fileReader.readAsArrayBuffer(blob);
}

function downloadFile()
{
    var fileName = "file1.txt";
    $.ajax({
        url: "http://localhost/Server/ashx/FileProcess.ashx",
        type: "POST",
        data: {
            uploadType: "download"
        },
        //dataType: "",
        success: function (data) {
            var fileContent = window.atob(data);
            saveFile(fileName, fileContent);
        },
        error: function (e) {
            alert(e.responseText);
        }
    });
}

function saveFile(fileName, fileContent) {
    var byteArr = new Array(fileContent.length);
    for (var i = 0; i < fileContent.length; i++) {
        byteArr[i] = fileContent.charCodeAt(i);
    }

    var blob = new Blob([new Uint8Array(byteArr)], { type: "application/octet-stream" });
    var url = window.URL.createObjectURL(blob);

    var a = document.createElement("a");
    if ("download" in a) { // 支持download属性
        document.body.appendChild(a);
        a.style = "display:none";
        a.href = url;
        //download属性IE不支持。。。
        a.download = fileName;
        a.click(); // 触发下载
        //revokeObjectURL会导致firefox不能下载。。。
        //window.URL.revokeObjectURL(url);
        document.body.removeChild(a);
    }
    else { //IE 10+
        if (typeof navigator !== "undefined" && navigator.msSaveOrOpenBlob) {
            return navigator.msSaveOrOpenBlob(blob, name);
        }
    }
}

$(document).ready(function () {
    $(document).on("change", "#filePicker", onFileUpload);
    $(document).on("change", "#largeFilePicker", onLargeFileUpload);
    $(document).on("click", "#downloadFileBtn", downloadFile);
});