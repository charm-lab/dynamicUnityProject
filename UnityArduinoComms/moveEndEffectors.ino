/*Module Level Variables*/
double arcLength; //arc length of gear travel

double degInc1 = 0.00; //degree increment for servo1
double degInc2 = 0.00; //degree increment for servo2

bool servo1Condition;
bool servo2Condition;

/*********************************************************
   Function moveEndEffector
   Parameters: distanceToMove, thetaCurrent, finalPosition
   Returns: void
   Purpose: Extends the end effector by the input value relative
            to the servo's initial position. 
            Updates servo position after move is competed.
*/
void moveEndEffectors(double distanceToMove1, double thetaCurrent1, double finalPosition1, double distanceToMove2, double thetaCurrent2, double finalPosition2) {

  /*Servo1*/
  bool continueServo1 = true;
  double deltaTheta1 = (distanceToMove1 / PITCH_RADIUS) * (180 / PI); //Change in angle to meet new position
  double thetaFinal1 = thetaCurrent1 + deltaTheta1; //Final angle to go to to meet distance requested
  
  /*Servo2*/
  bool continueServo2 = true;
  double deltaTheta2 = (distanceToMove2 / PITCH_RADIUS) * (180 / PI); //Change in angle to meet new position
  double thetaFinal2 = thetaCurrent2 + deltaTheta2; //Final angle to go to to meet distance requested

  /*Move Servos:*/
  while(continueServo1 == true || continueServo2 == true){
    /*Servo1*/
    if(continueServo1 == true){
      if(distanceToMove1 > 0){ 
        servo1Condition = (theta1 <= thetaFinal1);
        degInc1 = 1;
      }
      else if(distanceToMove1 < 0){
         servo1Condition = (theta1 >= abs(thetaFinal1));
         degInc1 = -1;
      }
      else {
        servo1Condition = false;
      }
      
      if (servo1Condition){
        Servo1.write(theta1); // write to servo
        //delay(5); //  give servo time  to move
       
        theta1 +=degInc1; //update theta1
      }
      else{
        continueServo1 = false;
        initialPosition1 = finalPosition1; //Update position after moving completed
      }
    }
    
    /*Servo2*/
    if(continueServo2 == true){
      if(distanceToMove2 > 0){ 
        servo2Condition = (theta2 <= thetaFinal2);
        degInc2 = 1;
      }
      else if(distanceToMove2 < 0){
         servo2Condition = (theta2 >= abs(thetaFinal2));
         degInc2 = -1;
      }
      else {
        servo2Condition = false;
      }
      
      if (servo2Condition){
        Servo2.write(theta2); // write to servo
        //delay(5); //  give servo time  to move
       
        theta2 += degInc2; //update theta2
      }
      else{
        continueServo2 = false;
        initialPosition2 = finalPosition2; //Update position after moving completed
      }
    }
  }
}
