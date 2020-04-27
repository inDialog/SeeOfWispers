const WebSocket = require('ws')

const wss = new WebSocket.Server({port: 8000})
const path = require('path');
const fs = require('fs');

const keysGen = require('./keyGen.js')
const directoryPath = path.join(__dirname, 'Save');

var players = {};
var messageS = {};


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
		if(_data.includes('color'))
	    {
			CreatePlayer (_data,client)
			broadcastTextMesege ();
			broadcastArtWork ();
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
	   		ArtWork (data);
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

function ArtWork (data){
	var [udid,
		X, Y, Z,
		sX, sY, sZ,
		pX, pY, pZ,
		rX,rY,rZ,
		inUrl,inDescription] =  data.toString().split('\t');

	var artWroks ={};
	artWroks[udid] = {
			position: {
	    		x: parseFloat(X),
	        	y: parseFloat(Y),
	        	z: parseFloat(Z)
	      	},	
	      	artworkScale: {
	    		x: parseFloat(sX),
	        	y: parseFloat(sY),
	        	z: parseFloat(sZ)
	      	},
	      	platform: {
	    		x: parseFloat(pX),
	        	y: parseFloat(pY),
	        	z: parseFloat(pZ)
	      	},
	    	rotation: {
	        	x: parseFloat(rX),
	       		y: parseFloat(rY),
	        	z: parseFloat(rZ)
	      	},
	      	url : inUrl,
	    	description : inDescription,
	    	id : udid
	  }
	keysGen.LookForKey(udid, function(response){
	const jsonString = JSON.stringify(artWroks, null, 2)
	var adress = './Save/' + udid + '.json'
	fs.writeFile(adress, jsonString, err => {
    	if (err) {
    	    console.log('Error writing file', err)
    	} else {
        	console.log('Successfully wrote file')
    	}	
	})
		broadcastArtWork ();
		// console.log(response);
			})
}

function broadcastArtWork () {
  // broadcast messages to all clients
  wss.clients.forEach(function each (client) {
   if (client.readyState !== WebSocket.OPEN) 
    {
    	console.log('Client deleted');
    	return
    }
    fs.readdir(directoryPath, function (err, files) {
    if (err) {
        return console.log('Unable to scan directory: ' + err);
    } 
    files.forEach(function (file) {
    	var [id,type] =file.toString().split('.');
    	if('json'==type){
    		    jsonReader('./Save/' +  file, (err, recide) => {
   					 if (err) {
       					 console.log(err)
        				return
    					}
    				// console.log("Send" ,recide)
    				var temp =  Object.keys(recide).map(udid => recide[udid])
    				client.send(JSON.stringify({artWroks: temp}))
				})
        // console.log(file.toString()); 
    }
    });
});

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

function jsonReader(filePath, cb) {
    fs.readFile(filePath, (err, fileData) => {
        if (err) {
            return cb && cb(err)
        }
        try {
            const object = JSON.parse(fileData)
            return cb && cb(null,object)
        } catch(err) {
            return cb && cb(err)
        }
    })
}


// call broadcastUpdate every 0.1s
setInterval(broadcastUpdate, 200);
