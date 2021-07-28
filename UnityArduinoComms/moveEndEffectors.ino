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
  
  /*Status Print Statements 
  Serial.println("\tDelta Theta1: " + (String)deltaTheta1 +"\t|\t"+ "Delta Theta2: " + (String)deltaTheta2 + "\n"+
                 "\tTheta Final1: " + (String)thetaFinal1 +"\t|\t"+ "Theta Final2: " + (String)thetaFinal2 + "\n");
  delay(PRINT_DELAY);
  */
  
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
        /*Serial.println("DegInc1: " + (String)degInc1);//DEBUG
        delay(PRINT_DELAY);*/
      }
      else{
        continueServo1 = false;
        initialPosition1 = finalPosition1; //Update position after moving completed
        /*
        Serial.println("******FINISHED servo1");
        delay(PRINT_DELAY);
        */
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
        /*
        Serial.println("DegInc2: " + (String)degInc2); //DEBUG
        delay(PRINT_DELAY);
        */
      }
      else{
        continueServo2 = false;
        initialPosition2 = finalPosition2; //Update position after moving completed
        /*
        Serial.println("******FINISHED servo2");
        delay(PRINT_DELAY);
        */
      }
    }
    /*
    Serial.println("continue servo1: " + (String)continueServo1); delay(10);
    Serial.println("continue servo2: " + (String)continueServo2); delay(10);
    Serial.println("END OF WHILE");
    delay(PRINT_DELAY);
    */
  }
  /*
  Error Values - May need recalculation
  double errorVal1 = abs(finalPosition1 - (theta1 * PITCH_RADIUS / (180 / PI))); //currentPosition = (theta * PITCH_RADIUS / (180 / PI));
  double errorVal2 = abs(finalPosition2 - (theta2 * PITCH_RADIUS / (180 / PI)));

  Serial.println("\tError1 = " + (String)errorVal1 +"\t|\t"+ "Error2 = " + (String)errorVal2); delay(PRINT_DELAY);//debug 

  
  checkForReset(finalPosition1, errorVal1, 1);   //Check if reset is needed for servo1 before rerunning code
  checkForReset(finalPosition2, errorVal2, 2);   //Check if reset is needed for servo2 before rerunning code
  */
}
