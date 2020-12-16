src="https://code.jquery.com/jquery-1.9.1.min.js"
src="https://cdnjs.cloudflare.com/ajax/libs/jquery.tabslet.js/1.7.3/jquery.tabslet.min.js"

//註冊登入tab切換

 $(function(){

var _showTab = 0;
var $defaultLi = $('ul.tab-group li').eq(_showTab).addClass('active');
$($defaultLi.find('a').attr('href')).siblings().hide();


$('ul.tab-group li').click(function() {
    
    var $this = $(this),
     _clickTab = $this.find('a').attr('href');
    $this.addClass('active').siblings('.active').removeClass('active');
    $(_clickTab).stop(false, true).fadeIn().siblings().hide();
    return false;
}).find('a').focus(function(){
    this.blur();
});
});




//註冊 欄位輸入限制

$(function(){ 

  $(":input.required").blur(function(){ 

    //username

    if($(this).is("#Username")){ 
      $(".formtip").remove(); 
      if(this.value==""||this.value.length>20||this.value==$("#Account").val()){ 
       var errMsg="<span class='formtip'><font color=#6FCCC1 ; font size=2>*必填欄位且不可與帳號相同</font></span>"; 
       $(this).parent().append(errMsg); 
     }
     else{ 
      $(".formtip").remove();
     } 
   }
   
   //account

   if($(this).is("#Account")){ 
    $(".emailtip").remove(); 
    var reg =  /^.+@.+\..{2,3}$/; 
    var $email=$("#Account").val(); 
       if (!reg.test($email) || this.value.length > 50){ 
      var errMsg="<span class='emailtip'><font color=#6FCCC1 ; font size=2>*請輸入正確郵箱格式</font></span>"; 
      $(this).parent().append(errMsg); 
      return false;
    }
    else{ 
      $(".emailtip").remove(); 
    } 
  }
  
   //password

  if($(this).is("#Password")){ 
    $(".passwordtip").remove(); 
    var reg =/^(?=^.{8,}$)((?=.*[A-Za-z0-9])(?=.*[A-Z])(?=.*[a-z]))^.*$/; 
    var $password=$("#Password").val(); 
      if (!reg.test($password) || this.value.length > 100){ 
      var errMsg="<span class='passwordtip' ><font color=#6FCCC1 ; font size=2>*請輸入大小寫英文字母及數字</font></span>"; 
      
      $(this).parent().append(errMsg); 
      return false;
    }
    else{ 
      $(".passwordtip").remove(); 
    } 
  } 

    //repeatpassword

  if($(this).is("#PasswordCheck")){ 
    $(".repeatpasswordtip").remove(); 
    var $password=$("#Password").val(); 
    if(this.value!=$password||this.value==""){ 
     var errMsg="<span class='repeatpasswordtip'><font color=#6FCCC1 ; font size=2>*密碼不一致</font></span>"; 
     $(this).parent().append(errMsg); 
     return false;
   }

   else{ 
    $(".repeatpasswordtip").remove();
   } 
  }
  
    //gender

  if($(this).is("#gender")){ 
    $(".gendertip").remove(); 

      if (document.signupform.Gender.value == "0" )
   {
    var errMsg="<span class='gendertip'><font color=#6FCCC1 ; font size=2>*必須選填</font></span>"; 
    $(this).parent().append(errMsg); 
   }
   else{ 
    $(".gendertip").remove();
   } 
}

   
})
}) 







//登入 輸入欄位限制 


$(function(){ 

  $(":input.required").blur(function(){ 




 //account

 if($(this).is("#Account2")){ 
  $(".emailtip").remove(); 
  var reg =  /^.+@.+\..{2,3}$/; 
  var $email2=$("#Account2").val(); 
  if(!reg.test($email2)){ 
    var errMsg="<span class='emailtip'><font color=#6FCCC1 ; font size=2>*請輸入正確郵箱格式</font></span>"; 
    $(this).parent().append(errMsg); 
    return false;
  }
  else{ 
    $(".emailtip").remove(); 
  } 
}

 //password

if($(this).is("#Password2")){ 
  $(".passwordtip").remove(); 
  var reg =/^(?=^.{8,}$)((?=.*[A-Za-z0-9])(?=.*[A-Z])(?=.*[a-z]))^.*$/; 
  var $password2=$("#Password2").val(); 
  if(!reg.test($password2)){ 
    var errMsg="<span class='passwordtip' ><font color=#6FCCC1 ; font size=2>*請輸入大小寫英文字母及數字</font></span>"; 
    
    $(this).parent().append(errMsg); 
    return false;
  }
  else{ 
    $(".passwordtip").remove(); 
  } 
} 


   
})
}) 










//註冊 全部填寫完才可submit
$(document).ready(function () {
           
  $("#getstart").click(function(){
      if($("#Username").val()==false){
          alert("尚未完成username填寫");
          eval("document.signupform['username'].focus()");       
      }else if($("#Account").val()==false){
          alert("尚未完成信箱填寫");
          eval("document.signupform['Account'].focus()");    
      }else if($("#Password").val()==false){
          alert("尚未完成密碼填寫");
          eval("document.signupform['Password'].focus()");       
      }else{
          document.signupform.submit();

      }
  })
})


//登入 全部填寫完才可submit

$(document).ready(function(){
  $("#loginbtn").click(function(){
if($("#Account2").val()==false){
          alert("尚未完成信箱填寫");
          eval("document.loginform['Account2'].focus()");    
      }else if($("#Password2").val()==false){
          alert("尚未完成密碼填寫");
          eval("document.loginform['Password2'].focus()");       
      }else{
          document.loginform.submit();
        }
  })
})






 






