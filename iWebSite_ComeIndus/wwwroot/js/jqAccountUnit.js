﻿src = "https://code.jquery.com/jquery-1.9.1.min.js"
src = "https://cdnjs.cloudflare.com/ajax/libs/jquery.tabslet.js/1.7.3/jquery.tabslet.min.js"

//Ready 一開始載入
$(document).ready(function () {
    //登入按鈕 事件
    $("#loginbtn").click(function () {
        //帳號與密碼檢測
        if (AccountCheck() & PasswordCheck()) {
            //送出表單 登入
            document.loginform.submit();
        } else {
            //錯誤訊息
            alert("帳號或密碼不符合規定。");
        }
    })

    //註冊按鈕 事件
    $("#getstart").click(function () {
        //註冊條件檢查 & BirthdayCheck()
        if (UsernameCheck() &
            AccountCheck() &
            PasswordCheck() &
            RepeatPwdCheck() &
            GenderCheck()) {
            //送出表單 註冊
            document.signupform.submit();
        } else {
            //錯誤訊息
            alert("註冊資料不符合規定。");
        }
    })

    //修改密碼 事件
    $("#ResetPwd").click(function () {
        //註冊條件檢查 & BirthdayCheck()
        if (PasswordCheck() &
            RepeatPwdCheck()) {
            //送出表單 修改密碼
            document.ResetForm.submit();
        } else {
            //錯誤訊息
            alert("新密碼不符合規定。");
        }
    })

    //當欄位失去焦點時觸發 欄位限制檢查
    $(":input.required").blur(function () {
        //使用者姓名 Username
        if ($(this).is("#Username")) {
            UsernameCheck();
        }

        //帳號 Account
        if ($(this).is("#Account")) {
            AccountCheck();
        }

        //密碼 Password
        if ($(this).is("#Password")) {
            PasswordCheck();
        }

        //再輸入一次密碼 RepeatPwd
        if ($(this).is("#PasswordCheck")) {
            RepeatPwdCheck();
        }

        //性別 Gender
        if ($(this).is("#Gender")) {
            GenderCheck();
        }

        ////生日 Birthday
        //if ($(this).is("#Birthday")) {
        //    BirthdayCheck();
        //}
    })
})

//使用者姓名檢測
function UsernameCheck() {
    //錯誤訊息移除
    $(".formtip").remove();

    //使用者帳號物件
    var Username = $("#Username");

    //條件檢查
    if (Username.val() == "" || Username.val().length > 20 || Username.val() == $("#Account").val()) {
        //錯誤訊息
        var errMsg = "<span class='formtip'><font color=#6FCCC1 ; font size=2>*必填欄位且不可與帳號相同</font></span>";

        //訊息顯示
        Username.parent().append(errMsg);

        //Return 失敗
        return false;
    }

    //Return 成功
    return true;
}

//帳號檢測
function AccountCheck() {
    //錯誤訊息移除
    $(".emailtip").remove();

    //帳號限制條件
    var AccountReg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;

    //帳號物件
    var Account = $("#Account");

    //條件檢查
    if (!AccountReg.test(Account.val()) || Account.val().length > 50) {
        //錯誤訊息
        var errMsg = "<span class='emailtip'><font color=#6FCCC1 ; font size=2>*請輸入正確郵箱格式</font></span>";

        //顯示錯誤訊息
        Account.parent().append(errMsg);

        //Return 失敗
        return false;
    }
    //Return 成功
    return true;
}

//密碼檢測
function PasswordCheck() {
    //錯誤訊息移除
    $(".passwordtip").remove();

    //密碼限制條件
    var PasswordReg = /^(?=.*[a-zA-Z])(?=.*\d)(?=.*[~!@#$%^&*()_ `\-={}:";'<>?,.\/]).{8,20}$/;

    //密碼
    var Password = $("#Password");

    //條件檢查
    if (!PasswordReg.test(Password.val())) {
        //錯誤訊息
        var errMsg = "<span class='passwordtip' ><font color=#6FCCC1 ; font size=2>*請輸入8-20碼的大小寫英文字母及數字符號</font></span>";

        //顯示錯誤訊息
        Password.parent().append(errMsg);

        //Return 失敗
        return false;
    }
    //Return 成功
    return true;
}

//再輸入一次密碼檢測
function RepeatPwdCheck() {
    //移除錯誤訊息
    $(".repeatpasswordtip").remove();

    //再輸入一次密碼 物件 
    var RepeatPwd = $("#PasswordCheck");

    //條件限制
    if (RepeatPwd.val() == "" || RepeatPwd.val() != $("#Password").val()) {
        //錯誤訊息
        var errMsg = "<span class='repeatpasswordtip'><font color=#6FCCC1 ; font size=2>*密碼為空或不一致</font></span>";

        //錯誤訊息顯示
        RepeatPwd.parent().append(errMsg);

        //Return 失敗
        return false;
    }
    //Return 成功
    return true;
}

//性別檢測
function GenderCheck() {
    //錯誤訊息移除
    $(".gendertip").remove();

    //性別 物件
    var Gender = $("#Gender");

    //條件限制
    if (Gender.val() == "0") {
        //錯誤訊息
        var errMsg = "<span class='gendertip'><font color=#6FCCC1 ; font size=2>*必須選填</font></span>";

        //錯誤訊息顯示
        Gender.parent().append(errMsg);

        //Return 失敗
        return false;
    }
    //Return 成功
    return true;
}

//生日檢測
function BirthdayCheck() {
    //錯誤訊息移除
    $(".BirthdayTip").remove();

    //性別 物件
    var Birthday = $("#Birthday");

    //條件限制
    if (Birthday.val() == "") {
        //錯誤訊息
        var errMsg = "<span class='gendertip'><font color=#6FCCC1 ; font size=2>*必須選填</font></span>";

        //錯誤訊息顯示
        Birthday.parent().append(errMsg);

        //Return 失敗
        return false;
    }
    //Return 成功
    return true;
}