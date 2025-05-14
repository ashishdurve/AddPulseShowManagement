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
        url: "../Cracked/AddScreen",
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
    //if (ScreenNo == "1")
    //    window.location.href = "../GameEditor/Scr2Title?Gameid=" + GameID;

    //if (ScreenNo == "2")
    //    window.location.href = "../GameEditor/Scr3CreateGame?Gameid=" + GameID;

    //if (ScreenNo == "3")
    //    window.location.href = "../GameEditor/Scr4CreateGameSetup?Gameid=" + GameID;

    //if (ScreenNo == "4")
    //    window.location.href = "../GameEditor/Scr5SelectName?Gameid=" + GameID;

    //if (ScreenNo == "5")
    //    window.location.href = "../GameEditor/Scr6SelectDetective?Gameid=" + GameID;

    //if (ScreenNo == "6")
    //    window.location.href = "../GameEditor/Scr7WaitingDetectivesInvisible?Gameid=" + GameID;

    //if (ScreenNo == "7")
    //    window.location.href = "../GameEditor/Scr7WaitingDetectivesVisible?Gameid=" + GameID;

    //if (ScreenNo == "8")
    //    window.location.href = "../GameEditor/Scr8WaitingDetectives?Gameid=" + GameID;

    //if (ScreenNo == "9")
    //    window.location.href = "../GameEditor/Scr9DetectiveOne?Gameid=" + GameID;

    //if (ScreenNo == "10")
    //    window.location.href = "../GameEditor/Scr9DetectiveTwo?Gameid=" + GameID;

    //if (ScreenNo == "11")
    //    window.location.href = "../GameEditor/Scr9DetectiveThree?Gameid=" + GameID;

    //if (ScreenNo == "12")
    //    window.location.href = "../GameEditor/Scr9DetectiveFour?Gameid=" + GameID;

    //if (ScreenNo == "13")
    //    window.location.href = "../GameEditor/Scr9DetectiveFive?Gameid=" + GameID;

    //if (ScreenNo == "14")
    //    window.location.href = "../GameEditor/Scr11Magnifying?Gameid=" + GameID;

    //if (ScreenNo == "15")
    //    window.location.href = "../GameEditor/Scr12CorrectGuess?Gameid=" + GameID;

    //if (ScreenNo == "16")
    //    window.location.href = "../GameEditor/Scr13InCorrectGuess?Gameid=" + GameID;

    //if (ScreenNo == "17")
    //    window.location.href = "../GameEditor/Scr14Ranking?Gameid=" + GameID;

    //if (ScreenNo == "18")
    //    window.location.href = "../GameEditor/Scr15Winner?Gameid=" + GameID;

    //if (ScreenNo == "19")
    //    window.location.href = "../GameEditor/Scr16FinalScore?Gameid=" + GameID;

    //if (ScreenNo == "20")
    //    window.location.href = "../GameEditor/Scr17JoiningGame?Gameid=" + GameID;

    //if (ScreenNo == "21")
    //    window.location.href = "../GameEditor/Scr18Multiplayer?Gameid=" + GameID;

    //if (ScreenNo == "22")
    //    window.location.href = "../GameEditor/Scr19MultiplayerPlacementSecond?Gameid=" + GameID;

    //if (ScreenNo == "23")
    //    window.location.href = "../GameEditor/Scr19MultiplayerPlacementThird?Gameid=" + GameID;

    //if (ScreenNo == "24")
    //    window.location.href = "../GameEditor/Scr19MultiplayerPlacementFourth?Gameid=" + GameID;

    //if (ScreenNo == "25")
    //    window.location.href = "../GameEditor/Scr19MultiplayerPlacementFifth?Gameid=" + GameID;

    //if (ScreenNo == "26")
    //    window.location.href = "../GameEditor/Scr20ForgerWin?Gameid=" + GameID;

    //if (ScreenNo == "27")
    //    window.location.href = "../GameEditor/Scr21NewTutorialFirst?Gameid=" + GameID;

    //if (ScreenNo == "28")
    //    window.location.href = "../GameEditor/Scr21NewTutorialSecond?Gameid=" + GameID;

    //if (ScreenNo == "29")
    //    window.location.href = "../GameEditor/Scr21NewTutorialThird?Gameid=" + GameID;

    //if (ScreenNo == "30")
    //    window.location.href = "../GameEditor/Scr21NewTutorialFourth?Gameid=" + GameID;

    //if (ScreenNo == "31")
    //    window.location.href = "../GameEditor/Scr22Credits?Gameid=" + GameID;

    //if (ScreenNo == "32")
    //    window.location.href = "../GameEditor/Scr22Settings?Gameid=" + GameID;

    //if (ScreenNo == "33")
    //    window.location.href = "../GameEditor/Scr10Painting?Gameid=" + GameID;

    //if (ScreenNo == "34")
    //    window.location.href = "../GameEditor/Scr10PaintingMunch?Gameid=" + GameID;

    //if (ScreenNo == "35")
    //    window.location.href = "../MuseumUser/MyGameOne?Gameid=" + GameID;

    if (ScreenNo == "0")
        DisplayMessage(ErrorCross + SpanStart + "Something Went Wrong!" + SpanEnd);

    if (ScreenNo == "")
        DisplayMessage(ErrorCross + SpanStart + "Something Went Wrong!" + SpanEnd);

    if (ScreenNo == "01")
        window.location.href = "../Cracked/Scr2CameraFillScreen?Gameid=" + GameID;

    if (ScreenNo == "02")
        window.location.href = "../Cracked/Scr3GameplayWaterScreenOnePartA?Gameid=" + GameID;
    
    if (ScreenNo == "03")
        window.location.href = "../Cracked/Scr3GameplayWaterScreenOne?Gameid=" + GameID;

    if (ScreenNo == "04")
        window.location.href = "../Cracked/Scr4GameplayWaterScreenTwo?Gameid=" + GameID;

    if (ScreenNo == "05")
        window.location.href = "../Cracked/Scr5GameplayWaterScreenThree?Gameid=" + GameID;

    if (ScreenNo == "06")
        //window.location.href = "../GameEditor/Scr21NewTutorialSecond?Gameid=" + GameID;
        window.location.href = "../MuseumUser/MyGameOne?Gameid=" + GameID;
}