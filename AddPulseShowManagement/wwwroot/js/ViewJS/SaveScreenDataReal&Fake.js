function SaveData(GameID, ScreenNo, data1, data2, data3, data4, data5, data6, data7, data8, data9, data10, data11, redirect) {
    var data1 = data1
    var data2 = data2;
    var data3 = data3;
    var data4 = data4;
    var data5 = data5;
    var data6 = data6;
    var data7 = data7;
    var data8 = data8;
    var data9 = data9;
    var data10 = data10;
    var data11 = data11;

    var values;
    var datas = [];

    values = {
        "data": data1
    }
    datas.push(values);

    values = {
        "data": data2
    }
    datas.push(values);

    values = {
        "data": data3
    }
    datas.push(values);

    values = {
        "data": data4
    }
    datas.push(values);

    values = {
        "data": data5
    }
    datas.push(values);

    values = {
        "data": data6
    }
    datas.push(values);

    values = {
        "data": data7
    }
    datas.push(values);

    values = {
        "data": data8
    }
    datas.push(values);

    values = {
        "data": data9
    }
    datas.push(values);

    values = {
        "data": data10
    }
    datas.push(values);

    values = {
        "data": data11
    }
    datas.push(values);

    var ScreenData = JSON.stringify(datas);
    var GameID = GameID;
    var ScreenNo = ScreenNo;

    var FullData = {
        "GameID": GameID,
        "ScreenNo": ScreenNo,
        "ScreenData": ScreenData
    }

    $.ajax({
        type: "POST",
        url: "../RealFake/AddScreen",
        data: FullData,
        success: function (data) {

            DisplayMessage(SuccessCheck + SpanStart + 'Screen ' + ScreenNo + ' Added successfully!' + SpanEnd);

            if (redirect) {
                setTimeout(function () {
                    RedirectToURL(ScreenNo, GameID)
                }, 500);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            DisplayMessage(ErrorCross + SpanStart + "Something Went Wrong!" + SpanEnd);
        }
    });
}

function RedirectToURL(ScreenNo, GameID) {
    if (ScreenNo == "1")
        window.location.href = "../RealFake/Scr02_NameSelection?Gameid=" + GameID;

    if (ScreenNo == "2")
        window.location.href = "../RealFake/Scr03_IntroCutscenePart1?Gameid=" + GameID;

    if (ScreenNo == "3")
        window.location.href = "../RealFake/Scr04_IntroCutscenePart2?Gameid=" + GameID;

    if (ScreenNo == "4")
        window.location.href = "../RealFake/Scr05_IntroCutscenePart3?Gameid=" + GameID;

    if (ScreenNo == "5")
        window.location.href = "../RealFake/Scr07_Gameplay?Gameid=" + GameID;

    if (ScreenNo == "6")
        window.location.href = "../RealFake/Scr08_FinalScoreVictory?Gameid=" + GameID;

    if (ScreenNo == "7")
        window.location.href = "../RealFake/Scr09_FinalScoreDefeat?Gameid=" + GameID;

    if (ScreenNo == "8")
        window.location.href = "../RealFake/Scr06_GoToPainting?Gameid=" + GameID;

    if (ScreenNo == "34")
        window.location.href = "../RealFake/Scr10PaintingMunch?Gameid=" + GameID;

    if (ScreenNo == "35")
        window.location.href = "../MuseumUser/MyGameOne?Gameid=" + GameID;

    if (ScreenNo == "0")
        DisplayMessage(ErrorCross + SpanStart + "Something Went Wrong!" + SpanEnd);

    if (ScreenNo == "")
        DisplayMessage(ErrorCross + SpanStart + "Something Went Wrong!" + SpanEnd);

    if (ScreenNo == "204")
        //window.location.href = "../RealFake/Scr21NewTutorialSecond?Gameid=" + GameID;
        window.location.href = "../MuseumUser/MyGameOne?Gameid=" + GameID;
}