%Unity Data Collection  - Beta Test

arduinoDesPos1 = report(1:end, 1);
arduinoDesPos2 = report(1:end, 2);
arduinoFinalPos1 = report(1:end, 3);
arduinoFinalPos2 = report(1:end, 4);
arduinoElapsedTime = report(1:end, 5);
unityCurrentTime = report(1:end, 6);

figure(1)
plot(unityCurrentTime, arduinoDesPos1); hold on;
plot(unityCurrentTime, arduinoDesPos2); hold on;
plot(unityCurrentTime, arduinoFinalPos1, "o"); hold on;
plot(unityCurrentTime, arduinoFinalPos2, "o"); 
%plot(unityCurrentTime, arduinoElapsedTime); hold on;
xlabel("unityCurrentTime [sec]");
ylabel("Position [mm]")
title("Arduino Information")
legend("arduinoDesPos1","arduinoDesPos2","arduinoFinalPos1",...
    "arduinoFinalPos2");%,"arduinoElapsedTime");
% improvePlot;

figure(2);
plot(unityCurrentTime,arduinoElapsedTime, "-o")
xlabel("unityCurrentTime [sec]");
ylabel("arduinoElapsedTime [ms]")
title("Arduino Timing Information")
legend("arduinoElapsedTime");
% improvePlot;
% 
% figure(3);
% plot(1:length(arduinoElapsedTime),arduinoElapsedTime)
% xlabel("index");
% ylabel("arduinoElapsedTime [ms]")
% title("Arduino Timing Information")
% legend("arduinoElapsedTime");
% improvePlot;



% Elapsed Time info
Mean = mean(arduinoElapsedTime)
Median = median(arduinoElapsedTime)
Minimum = min(arduinoElapsedTime)
Maximum = max(arduinoElapsedTime)
STD = std(arduinoElapsedTime)



