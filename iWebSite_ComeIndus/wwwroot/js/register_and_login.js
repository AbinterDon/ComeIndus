src="https://code.jquery.com/jquery-1.9.1.min.js"
src="https://cdnjs.cloudflare.com/ajax/libs/jquery.tabslet.js/1.7.3/jquery.tabslet.min.js"






//註冊 欄位輸入限制

$(function(){ 

  $(":input.required").blur(function(){ 

    //username

    if($(this).is("#Username")){ 
      $(".formtip").remove(); 
      if(this.value==""||this.value.length>20||this.value==$("#Account").val()){ 
       var errMsg="<span class='formtip'><font color=#6FCCC1 ; font size=2>*必填欄位且不可與帳號相同</font></span>"; 
          $(this).parent().append(errMsg); 
          return false;
     }
     else{ 
      $(".formtip").remove();
     } 
   }
   
   //account

   if($(this).is("#Account")){ 
    $(".emailtip").remove(); 
       var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/; 
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
      var reg = /^(?=.*[a-zA-Z])(?=.*\d)(?=.*[~!@#$%^&*()_ `\-={}:";'<>?,.\/]).{8,20}$/; 
    var $password=$("#Password").val(); 
      if (!reg.test($password)){ 
          var errMsg ="<span class='passwordtip' ><font color=#6FCCC1 ; font size=2>*請輸入8-20碼的大小寫英文字母及數字符號</font></span>"; 
          
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




//註冊 全部填寫完才可submit
$(document).ready(function () {
           
  $("#getstart").click(function(){
      if($("#Username").val()==false){
          alert("尚未完成Username填寫");
          eval("document.signupform['username'].focus()");       
      }else if($("#Account").val()==false){
          alert("尚未完成Account填寫");
          eval("document.signupform['Account'].focus()");    
      }else if($("#Password").val()==false){
          alert("尚未完成Password填寫");
          eval("document.signupform['Password'].focus()");       
      }else{
          document.signupform.submit();

      }
  })
})


//登入 全部填寫完才可submit

$(document).ready(function(){
  $("#loginbtn").click(function(){
if($("#Account").val()==false){
          alert("尚未完成Account填寫");
          eval("document.loginform['Account'].focus()");    
      }else if($("#Password").val()==false){
          alert("尚未完成Password填寫");
          eval("document.loginform['Password'].focus()");       
      }else{
          document.loginform.submit();
        }
  })
})






 






