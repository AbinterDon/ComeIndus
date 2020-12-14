
const pass = false;



/*圖片上傳*/
function listenUploadFile() {
    var uploadb = document.getElementById("uploadb");
    var inputfile = document.getElementById('inputFile');

    uploadb.addEventListener('click', () => {

        inputfile.click()

    })

    inputfile.addEventListener('change', handleFiles)

}

function handleFiles() {
    var fileList = this.files;
    showImg(fileList);
}

function showImg(thisimg) {
    var file = thisimg[0];

    if (window.FileReader) {
        var fr = new FileReader();
        var showimg = document.getElementById('showimg');
        fr.onloadend = (e) => {
            showimg.src = e.target.result;
        };
        fr.readAsDataURL(file);
        var upp = document.getElementById('inputFile');
        upp.style.opacity = 0;
        showimg.style.display = 'block';
    }

}




function checkName() {
    var userName = document.getElementById('Actualname');
    var nameCheck = document.getElementById('ActualnameCheck');
    userName.addEventListener('change', () => {
        var name = userName.value;
        if (name == '') {
            nameCheck.textContent = '你沒有名字? 請輸入你的名字'
        }
        else {
            nameCheck.textContent = '';
        }
    })


}

function checkPassword() {
    var password = document.getElementById("Password"); /*第一次輸入密碼*/
    var passCheck = document.getElementById("PasswordCheck"); /*第二次輸入密碼*/
    var passMessage = document.getElementById("passCheck"); /*不同時的警訊*/
    passCheck.addEventListener('change', () => {

        var pwdval = password.value;
        var pwdCheckValue = passCheck.value;

        if (pwdval != pwdCheckValue) {
            passMessage.textContent = "密碼輸入不一樣";
        }
        else {
            passMessage.textContent = "";
        }
    })

}



listenUploadFile();
checkName();
checkPassword();

