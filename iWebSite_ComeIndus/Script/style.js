//滑動離開頂部時就取消at_top的class
$(window).scroll(function(e){
  if ($(window).scrollTop()<=0)
    $(".navbar,.explore").addClass("at_top");
  else
    $(".navbar,.explore").removeClass("at_top");
});

//緩慢滑動
$(document).on('click', 'a', function(event){
    event.preventDefault();
    $('html, body').animate({
        scrollTop: $( $.attr(this, 'href') ).offset().top
    }, 500);
});

//偵測進入貓咪範圍就站起來
function detect_cat(cat_id,x){
  var catplace = $(cat_id).offset().left+$(cat_id).width()/2;
  if (Math.abs(x-catplace)<80)
    $(cat_id).css("bottom","0px");
  else
    $(cat_id).css("bottom","-50px");
}

//滑鼠移動時觸發的事件
$(window).mousemove(function(evt){
  var pagex = evt.pageX;
  var pagey = evt.pageY;
  
  //計算相對區域的位置
  var x = pagex-$("section#section_about").offset().left;
  var y = pagey-$("section#section_about").offset().top;
  
  //計算現在的y位置超過區域則隱藏
  if (y<0 ||　y>$("section#section_about").outerHeight())
    $("#cross").css("opacity",0);
  else
    $("#cross").css("opacity",1);
  
  // console.log(x);
  //更動指標位置
  $("#cross").css("left",x+"px");
  $("#cross").css("top",y+"px");
  
  //計算貓的中心位置
  var catplace = $("#cat").offset().left +$("#cat").width()/2;
  var cattop = $("#cat").offset().top;
  
  var img_url="http://awiclass.monoame.com/catpic/";
  
  //左方 / 右方 / 上方
  if (pagex<catplace - 50)
    $("#cat").attr("src",img_url+"cat_left.png");
  
  else if (pagex>catplace  + 50)
    $("#cat").attr("src",img_url+"cat_right.png");
  
  else
    $("#cat").attr("src",img_url+"cat_top.png");
  
  //左上 / 右上
  if (pagex<catplace - 50 && pagey< cattop)
    $("#cat").attr("src",img_url+"cat_lefttop.png");
  
  if (pagex>catplace + 50 && pagey< cattop)
    $("#cat").attr("src",img_url+"cat_righttop.png");
    
  //站起來的貓咪
  // console.log(x);
  detect_cat("#cat_pink",pagex);
  detect_cat("#cat_grey",pagex);
  detect_cat("#cat_yellow",pagex);
  
  //更新一些移動景物的位置
  $(".mountain").css("transform","translateX("+(x/-20+50)+"px)")
  
  //更新簡介中文字的飄浮位置
  $(".r1text").css("transform","translateX("+y/-5+"px)");
  $(".r2text").css("transform","translateX("+y/-10+"px)");
  $(".r3text").css("transform","translateX("+y/-12+"px)");
  
   //更新三角形
  $(".tri1").css("transform",
                 "translateX("+x/-5+"px) rotate(-15deg)");
  $(".tri2").css("transform",
                 "translateX("+x/-10+"px) rotate(-15deg)");
  $(".tri3").css("transform",
                 "translateX("+x/-12+"px) rotate(-15deg)");
  $(".tri4").css("transform",
                 "translateX("+x/-14+"px) rotate(-15deg)");
  $(".tri5").css("transform",
                 "translateX("+x/-16+"px) rotate(-15deg)");
});

//滑鼠移入國家放大