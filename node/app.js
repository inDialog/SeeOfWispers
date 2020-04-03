const WebSocket = require('ws')

// create new websocket server
const wss = new WebSocket.Server({port: 8000})

// empty object to store all players
var players = {}

// on new client connect
wss.on('connection', function connection (client) {
  // on new message recieved
  client.send("Conceted");
 	console.log('Client connected');
 
  client.on('message', function incoming (data) {
    // get data from string
    var _data = data.toString();
    // chack for disconect 
    if(_data.includes('Disconected')){
    	var [udid,msg] = data.toString().split('\t')
    	// delete players[udid];
    	console.log('Client deisconected');
		return;
    }
     else  if(_data.includes('color')){
       [udid,r,g,b] = data.toString().split('\t');
         players[udid] = {
          position: {},
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
      console.log('ID+color bind assinged'  , players[udid]);
    return;
       
    }
    // Last option is UpdatePosition 
    var [udid, X, Y, Z,] =  data.toString().split('\t')
      players[udid].position = {
             x: parseFloat(X),
             y: parseFloat(Y),
             z: parseFloat(Z)
      }
    console.log('Position update,  data:' , players[udid]);
  });


})

function broadcastUpdate () {
  // broadcast messages to all clients
  wss.clients.forEach(function each (client) {
    // filter disconnected clients
    if (client.readyState !== WebSocket.OPEN) 
    {
     //  delete players[client.udid];
    	// wss.clients.delete(client);
    	console.log('Client deleted');
    	return
    }
    var nPlayers ={};

    // filter out current player by client.udid
    var otherPlayers = Object.keys(players).filter(udid => udid !== client.udid)

    // console.log(otherPlayers);
    // create array from the rest
    var otherPlayersPositions = otherPlayers.map(udid => players[udid])
    /// remove unmoved position
    // otherPlayersPositions.forEach(function each (player)
    // {
    //   if(!player.moved)
    //   {
    //     delete otherPlayersPositions[player.udid]; ;
    //     console.log('NotMoved');
    //   }
    // })

    // send it
    client.send(JSON.stringify({players: otherPlayersPositions}))

  })
}

// call broadcastUpdate every 0.1s
setInterval(broadcastUpdate, 100)
