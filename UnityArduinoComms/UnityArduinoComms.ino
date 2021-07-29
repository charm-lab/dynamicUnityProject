#include <SerialCommand.h>
#include <Servo.h>
#include <SoftwareSerial.h>

Servo Servo1; // create servo object to control Servo1
Servo Servo2; // create servo object to control Servo2

/*Global Constant Variables*/
const double PITCH_RADIUS = 5.75; //[mm] one half the pitch diamter
const int digitalPinNum1 = 9; //Digital pin from VR computer - for Servo1
const int digitalPinNum2 = 10; //Digital pin from VR computer - for Servo2
const double REF_VALUE = 0.00; //Reference value of system | Triggers restart of code
const double ERROR_THRESHOLD = 1.00; //Maximum allowable value for position error
//const double maxDistanceExtend = 20.0; //max distance the pusher can move
//const double maxDistanceContract = 0.00; //max distance the pusher can move  after being extended

/*Other Global Variables*/
double degreeIncrement; // Controls how fast we want theta to change
int caseNum = 0; //Number corresponding to situation causinga reset
int numLoops = 1; // Number of iterations in code

/*Initialize Servo1*/
double initialPosition1 = 0.00; //Posititon to move end effector from
double finalPosition1 = 0.00; //Posititon to move end effector to
double theta1 = 0.00; // variable to store the servo position

/*Initialize Servo2*/
double initialPosition2 = 0.00; //Posititon to move end effector from
double finalPosition2 = 0.00; //Posititon to move end effector to
double theta2 = 0.00; // variable to store the servo position

const int PRINT_DELAY = 5;

/*Goal position values*/
double desiredPos1 = 0.00;
double desiredPos2 = 0.00;

/*Unity Variables & Timing*/
double unityCurrentTime;
double trialNumber = 1; //default

/*Arudino Timing*/
unsigned long startTime;
unsigned long currentTime;
unsigned long arduinoElapsedTime;

/********************************************************
   Function setup
   Parameters: void
   Returns: void
   Purpose: Initiaize values to 0 or reinitialize after ref value is sent.
            Called at start of code or when checkForReset sends error message.
*/
void setup() {

  Serial.begin(9600); //Initialize serial monitor w/ 9600 baud rate
  Serial.setTimeout(5); //set timeout to 100 ms
  Servo1.attach(digitalPinNum1);  // attaches the servo to the Servo1 object
  Servo2.attach(digitalPinNum2);  // attaches the servo to the Servo2 object

  pinMode(13, OUTPUT);

  //Set all servo1 values to 0
  initialPosition1 = 0.00;
  theta1 = 0.00;
  Servo1.write(theta1); //Start at servo zero position

  //Set all servo2 values to 0
  initialPosition2 = 0.00;
  theta2 = 0.00;
  Servo2.write(theta2); //Start at servo zero position

  while (!Serial) {
    //Blocking Code -- wait for serial port to connect
  }
  //Serial.println("Serial Port Connected");
}

void loop() {

  startTime = millis();

  //Receives command from Unity via Serial
  receiveSerial();
/*
  Serial.println(numLoops);
  Serial.println("_______________________Start Loop #" + (String)numLoops + "_______________________\n");
  delay(PRINT_DELAY);

  //Set the position command:
    if(numLoops != 1){
  //Set the desired position from serial input
  receiveSerial();

  Serial.println("desiredPos1: " + (String)desiredPos1);
  Serial.println("desiredPos2: " + (String)desiredPos2);
    }
    else {
      desiredPos1 = 0.00;
      desiredPos2 = 0.00;
    }
*/
  /*Servo1*/
  finalPosition1 = desiredPos1;

  double distanceToMove1 = finalPosition1 - initialPosition1;
  double thetaCurrent1 = theta1;

  /*Servo2*/
  finalPosition2 = desiredPos2;

  double distanceToMove2 = finalPosition2 - initialPosition2;
  double thetaCurrent2 = theta2;

  /*Status Print Statements
    Serial.print("Servo1 Final Position: " + (String)finalPosition1 + "\t|\t" + "Servo2 Final Position: " + (String)finalPosition2 + "\n"
              + "Servo1 Initial Position: "   + (String)initialPosition1 + "\t|\t" + "Servo2 Initial Position: "   + (String)initialPosition2 + "\n"
              + "Servo1 Distance to Move: " + (String)distanceToMove1 + "\t|\t" + "Servo2 Distance to Move: " + (String)distanceToMove2 + "\n"
              + "Servo1 ThetaCurrent: " + (String)thetaCurrent1 + "\t|\t" + "Servo2 ThetaCurrent: " + (String)thetaCurrent2 + "\n");
              delay(PRINT_DELAY);
  */
  moveEndEffectors(distanceToMove1, thetaCurrent1, finalPosition1, distanceToMove2, thetaCurrent2, finalPosition2);

  //Serial.println("______________________Finished Loop______________________\n");
  //delay(1);
  numLoops++;

  currentTime = millis();
  arduinoElapsedTime = currentTime - startTime;

  //Prep Data for Processing:
  Serial.println((String)finalPosition1 + "," + (String)finalPosition2 + "," + (String)arduinoElapsedTime);
  //Serial.println((String)desiredPos1 + "," + (String)desiredPos2 + "," + (String)arduinoElapsedTime + "," + (String)unityCurrentTime + "," + (String)trialNumber);

}

/********************************************************
   Function receiveSerial
   Parameters: none
   Returns: void
   Purpose: Receives serial input and converts to usable form
            Can also test serial connnection if needed
            Sets the desired positions of each servo
*/

void receiveSerial() {
  if (Serial.available() > 0) {
    digitalWrite(13, HIGH);
    String rawData = Serial.readString();
    //Serial.println("Raw data: " + rawData);

    String inByte1 = rawData.substring(0, rawData.indexOf("A"));
    String inByte2 = rawData.substring(rawData.indexOf("A") + 1, rawData.indexOf("B"));
    /*
    String inByte3 = rawData.substring(rawData.indexOf("B") + 1, rawData.indexOf("C"));
    String inByte4 = rawData.substring(rawData.indexOf("C") + 1, rawData.indexOf("D"));
    */
  /*
    Serial.println("raw inByte1: " + inByte1);
    Serial.println("raw inByte2: " + inByte2);
    Serial.println("raw inByte3: " + inByte3);
    Serial.println("raw inByte4: " + inByte4);
    Serial.println("inByte1: " + (String)inByte1.toDouble());
    Serial.println("inByte2: " + (String)inByte2.toDouble());
    Serial.println("inByte3: " + (String)inByte3.toDouble());
    Serial.println("inByte4: " + (String)inByte4.toDouble());
  */
   
    desiredPos1 = inByte1.toDouble();
    desiredPos2 = inByte2.toDouble();
    /*
    unityCurrentTime = inByte3.toDouble();
    trialNumber = inByte4.toDouble();
    */
    //Serial.println("ARDUINO --> desiredPos1: " + (String)desiredPos1 + " desiredPos2: " + (String)desiredPos2);
  }
}
