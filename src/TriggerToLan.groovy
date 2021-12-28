
definition(
    name: "Trigger to HUB",
    namespace: "smartthings",
    author: "Neville",
    description: "Triggers and sends to raspberry",
    category: "Convenience",
    iconUrl: "http://iconbug.com/data/00/64/056647d3c72f7e4e54e3ac9df78d0474.png",
    iconX2Url: "http://iconbug.com/data/00/128/056647d3c72f7e4e54e3ac9df78d0474.png"
)

preferences {
	section(){
		input "bedroomMotionSensor", "capability.motionSensor"
        input "computerLight", "capability.switch"
	}
}

def setup(){
	subscribe(bedroomMotionSensor, "motion.active", bedroomTrigger)
	subscribe(bedroomMotionSensor, "motion.inactive", bedroomNoTrigger)
	subscribe(computerLight, "switch.on", computerLightTrigger)
	subscribe(computerLight, "switch.off", computerLightOffTrigger)
}

def computerLightTrigger(evt) {
	logEvent(evt, "ComputerLightOn")
}

def computerLightOffTrigger(evt) {
	logEvent(evt, "ComputerLightOff")
}


def bedroomTrigger(evt) {
	logEvent(evt, "BedroomMotion")
}

def bedroomNoTrigger(evt) {
	logEvent(evt, "BedroomNoMotion")
}

def installed() {
	log.debug "Installed with settings: ${settings}"
	setup()
}

def updated() {
	log.debug "Updated with settings: ${settings}"
	unsubscribe()
	setup()
}



def logEvent(evt, name) {
	log.debug evt
	lanCall("/logEvents/add/$name")
}

def lanCall(path)
{
	def ip4 = "192.168.0.28:5000"
    def ip = "192.168.0.140:5010"
	sendHubCommand(new physicalgraph.device.HubAction("""GET $path HTTP/1.1\r\nHOST: $ip\r\n\r\n""", physicalgraph.device.Protocol.LAN))
}