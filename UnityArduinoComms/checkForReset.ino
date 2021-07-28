/********************************************************
   Function checkForReset
   Parameters: finalPosition, errorVal, caseNum
   Returns: void
   Purpose: Decides if the code needs to reset.
            Reset conditions met if finalPosition = reference
            value or if error value exceeds threshold
            (allows overriding initialPosition = finalPosition if necessary)
            Informs user of what caused the reset using caseNum
*/
void checkForReset(double finalPosition, double errorVal,  int caseNum) {
  if (finalPosition == REF_VALUE || errorVal >= ERROR_THRESHOLD) {

    //Error Message:
    //Serial.println("    Refernce Value Sent \n~~~~~~~~~~~ CALLING SETUP | RESTARTING CODE ~~~~~~~~~~~~"); delay(PRINT_DELAY);

    caseNum = 0;
    delay(2000);
    setup(); //Call setup()
  }
}
