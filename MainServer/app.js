const WebSocket = require('ws')

// const wss = new WebSocket.Server({port: 8000})
const wss = new WebSocket.Server({port: 3002})

const path = require('path');
const fs = require('fs');

const keysGen = require('./keyGen.js')
const artManager = require('./ArtWorkManager.js')

const directoryPath = path.join(__dirname, 'Save');

var players = {};
var messageS = {};
module.exports.SendArtworks = SendArtworks;


// on new client connect
wss.on('connection', function connection (client) {
  // on new message recieved
 client.send("Conceted");
 console.log('Client connected');
 
	client.on('message', function incoming (data) 
		{
	    // get data from string
		var _data = data.toString();
		// console.log(_data)
	    // chack for handshake
		var temp={} ;

		if(_data.includes('color'))
	    {
		 
			SendArtworks(client,true)
			CreatePlayer (_data,client)
			broadcastTextMesege ();
	 		return;
	    }
	    if(_data.includes('RequestArtwork'))
	    {
			SendArtworks(client,true)
	   		return;
	    }
		if(_data.includes('TextMessage'))
	    {
	   		TextMesg (data);
			broadcastTextMesege ();
	   		return;
	    }
	    if(_data.includes('ArtWork'))
	    {
	   		// artManager.ArtWork(data);
	   		artManager.ArtWork(data, function(response){
    		// client.send(JSON.stringify("artKey@ "+ artistKey))
    		console.log(response);
        	SendArtworks(null,false);
			})
	   		return;
	    }
	     if(_data.includes('DeleteArtwork') )
	    {
	    	///TODO bettter way to get path  
	    	var [key,action] = _data.toString().split('\t');
	    	fs.unlink('./Save/'+key+'.json', (err) => {
	    		  if (err) {
    				console.error(err)
    				return
					}
	    	})
    		console.log("Delete " + key);

	    	return
	    }
	      if(_data ==  'GetKey' )
	    {
	    	var key =keysGen.createKey().toString();
    		client.send(JSON.stringify("artKey@" + key))
    		console.log(key);
	   		return;
	    }
	    if(_data.includes('CheckKey') )
	    {
	    	var[artistKey,type] = _data.toString().split('\t');
	    	keysGen.LookForKey(artistKey, function(response){
    		client.send(JSON.stringify("artKey@ "+ artistKey))
    		// console.log(response);
			})
	   		return;
	    }
	     if(_data.includes('players') )
	    {
    		console.log("UPdate");

	    }
	    // Last option is UpdatePosition 
	    UpdatePosition (data);
	    
  });
	client.on('close',function incoming (data) {
    	delete players[client.udid];
 		broadcastClose(client.udid); 
 		console.log("Closing");
	});

})

///TODO if(br) send only my artwork and not all the list 
function SendArtworks(client,br){
artManager.ComposeString(function(response){
    		var temp =  Object.keys(response).map(udid => response[udid])
    		if(br){
    		client.send(JSON.stringify({artWroks: temp}))
    		// console.log("solo"+temp);

    		}
    		else {
			broadcastArtWork (JSON.stringify({artWroks: temp}))
    		console.log("BR"+temp);

    		}
    		console.log(response);
			})
}


function broadcastArtWork (art) {
  // broadcast messages to all clients
  wss.clients.forEach(function each (client) {
   if (client.readyState !== WebSocket.OPEN) 
    {
    	console.log('Client deleted');
    	return
    }
   client.send(art)
   
  })
}

function TextMesg (data){
	var [udid, X, Y, Z,rX,rY,rZ,inTex] =  data.toString().split('\t');
		messageS[udid] = {
			position: {
	    		x: parseFloat(X),
	        	y: parseFloat(Y),
	        	z: parseFloat(Z)
	      	},	
	    	rotation: {
	        	x: parseFloat(rX),
	        	y: parseFloat(rY),
	        	z: parseFloat(rZ)
	      	},
	      	text : inTex,
	    	id : udid
	  	}
	// console.log('message,  data:' , messageS);
	}
function broadcastTextMesege () {
  // broadcast messages to all clients
  wss.clients.forEach(function each (client) {
   if (client.readyState !== WebSocket.OPEN) 
    {
    	console.log('Client deleted');
    	return
    }
    var otherPlayersPositions =  Object.keys(messageS).map(udid => messageS[udid])
    client.send(JSON.stringify({messageS: otherPlayersPositions}))
	// console.log('message,  data:' , otherPlayersPositions);
  })
}

function CreatePlayer (data,client){
	var [udid,r,g,b] = data.split('\t');
	players[udid] = {
	    position: {},
	    rotation: {},
	    color:{
	        r: parseFloat(r),
	        g: parseFloat(g),
	        b: parseFloat(b)
	      	},
	    timestamp: Date.now(),
	    id : udid,
	    moved : true
	 }
	 client.udid = udid
	// console.log('Position update,  data:' ,players);

}
function UpdatePosition (data){
	var [udid, X, Y, Z,rX,rY,rZ] =  data.toString().split('\t');
	if (typeof  players[udid] != "undefined") {
		players[udid].position = {
	    	x: parseFloat(X),
	        y: parseFloat(Y),
	        z: parseFloat(Z)
	      }
	    players[udid].rotation = {
	        x: parseFloat(rX),
	        y: parseFloat(rY),
	        z: parseFloat(rZ)
	      }
	// console.log('Position update,  data:' , players[udid]);
	}
}

function broadcastUpdate () {
  // broadcast messages to all clients
  wss.clients.forEach(function each (client) {
    // filter disconnected clients
    if (client.readyState !== WebSocket.OPEN) 
    {
    	console.log('Client deleted');
    	return
    }
    // filter out current player by client.udid
    var otherPlayers = Object.keys(players).filter(udid => udid !== client.udid)
    // create array from the rest
    var otherPlayersPositions = otherPlayers.map(udid => players[udid])
    client.send(JSON.stringify({players: otherPlayersPositions}))
  })
  // console.log(keysGen.createKey().toString());
  // console.log("updating");
}

function broadcastClose (id) {
  // broadcast messages to all clients
  wss.clients.forEach(function each (client) {
  	   if (client.readyState !== WebSocket.OPEN) 
    {
    	console.log('Client deleted');
    	return
    }
    var otherPlayers = Object.keys(players).filter(udid => udid !== client.udid)
    // create array from the rest
    var otherPlayersPositions = otherPlayers.map(udid => players[udid])
    client.send(`Deleted@${id}`);
    console.log(`Deleted ${id}`);
  })
}




// call broadcastUpdate every 0.1s
setInterval(broadcastUpdate, 200);
