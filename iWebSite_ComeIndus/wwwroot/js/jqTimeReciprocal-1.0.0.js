//----------------------------
// jqTimeReciprocal : 倒數時間，並控制Button狀態
// v 1.0.0 Abinter
//----------------------------
var jqTimeReciprocal = {
    //設定值
    settings: {},
    //初始值
    Init: function (options) {
        //設定
        var default_options = {
            //限制多少時間
            limitedTime: 60,

            //時間計數
            timeCount: 0,

            //倒數物件
            objClock: function () { },

            //按鈕Type
            objBtnType: "Input",

            //倒數按鈕
            objReciprocalBtn: "#reload_btn",

            //倒數完後按鈕復原名稱
            objOriginalBtn: "Sent again",

            //預備事件
            event_before_reload: function () { }
        };

        //Setting
        jqTimeReciprocal.settings = $.extend(default_options, options);

        //DoWork
        //jqTimeReciprocal.DoWork();
    },

    //DoWork
    DoWork: function () {
        //時間初始化
        jqTimeReciprocal.fn_initializeTime();

        //每秒跑一次countSecond
        objClock = setInterval("jqTimeReciprocal.fn_countSecond()", 1000);
    },

    //時間初始化 
    fn_initializeTime: function () {
        //時間計數 初始化
        jqTimeReciprocal.settings.timeCount = jqTimeReciprocal.settings.limitedTime;
    },

    //時間倒數Function (預設不改)
    fn_countSecond: function () {
        if (jqTimeReciprocal.settings.objBtnType == "Input") {
            if (jqTimeReciprocal.settings.timeCount > 0) {
                //按鍵不可用 與 時間遞減
                $(jqTimeReciprocal.settings.objReciprocalBtn).attr("value", "請" + jqTimeReciprocal.settings.timeCount-- + "秒後再試");
                $(jqTimeReciprocal.settings.objReciprocalBtn).attr("disabled", true);

            } else {
                //停止倒數
                clearInterval(jqTimeReciprocal.settings.objClock);

                //按鍵可用
                $(jqTimeReciprocal.settings.objReciprocalBtn).attr("value", "Sent again");
                $(jqTimeReciprocal.settings.objReciprocalBtn).attr("disabled", false);
            }
        } else {
            if (jqTimeReciprocal.settings.timeCount > 0) {
                //按鍵不可用 與 時間遞減
                $(jqTimeReciprocal.settings.objReciprocalBtn).text( "請" + jqTimeReciprocal.settings.timeCount-- + "秒後再試");
                $(jqTimeReciprocal.settings.objReciprocalBtn).attr("disabled", true);

            } else {
                //停止倒數
                clearInterval(jqTimeReciprocal.settings.objClock);

                //按鍵可用
                $(jqTimeReciprocal.settings.objReciprocalBtn).text(jqTimeReciprocal.settings.objOriginalBtn);
                $(jqTimeReciprocal.settings.objReciprocalBtn).attr("disabled", false);
            }
        }
    },

    //開始重整倒數
    fn_reloadStartUp: function () {},

    //停止重整倒數
    fn_reloadStop: function () {},

    //Reload (OverRide)
    fn_reload: function () {
        window.location.reload();
    }
};







