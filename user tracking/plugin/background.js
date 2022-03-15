// Declare color variable 
let color = '#3aa757';

chrome.runtime.onInstalled.addListener(() => {
  chrome.storage.sync.set({ color });
  console.log('Default background color set to %cgreen', `color: ${color}`);

});

// get current URL
chrome.tabs.onActivated.addListener( function(activeInfo){
    chrome.tabs.get(activeInfo.tabId, function(tab){
        y = tab.url;
        console.log("you are here: "+y);
		//callBackendApi(y);
    });
});

chrome.tabs.onUpdated.addListener((tabId, change, tab) => {
    if (tab.active && change.url) {
        console.log("you are here: "+change.url);
		//callBackendApi(change.url);
    }
});

function callBackendApi(y)
{
    let data = {
        "url" : y
    }
    let j_data = JSON.stringify(data);

    console.log(j_data)
    const url = "http://127.0.0.1:5000/";
    let xhr = new XMLHttpRequest();
    xhr.setRequestHeader('Content-type', 'application/json; charset=UTF-8');
    xhr.open('POST', url, true);
    xhr.send(j_data);

    xhr.onload = function () {
        if(xhr.status === 200) {
            alert("Post successfully created!");
        }
    }

    /*alert("function called"+y);
    $.ajax
    (
        {
            type: "POST",
            url: "http://127.0.0.1:5000/",
            data: y,
            success: function(msg)
            {
                console.log("URL Tracked");
                alert("URL Tracked");
            }//end function
        }
    );//End ajax */

}//end function