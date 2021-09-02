% Data Plotting for dynamicUnityProject Virtual Environment

clear; close all; clc;

folderPath = "C:\Users\Jasmin\Documents\OneDrive - Stanford\Unity Reseach Project\dynamicUnityProject\dynamicUnityProject_v3\Assets\Report_Tests\";
fileName = "report_08-24-2021 11-34-53-44";
fileToRead = folderPath + fileName + ".csv";

report = xlsread(fileToRead);

%Choose rows of interest
startRow = 1; endRow =  size(report,1);

% Variables to be plotted
time = report(startRow:endRow, 1);
sphereX = report(startRow:endRow, 18);
sphereY = report(startRow:endRow, 19);
sphereZ = report(startRow:endRow, 20);

figure(1);

improvePlot;

for i = 1:size(time,1) 
    
    % plot point
    plot3(sphereX(i),sphereZ(i),sphereY(i),"bo", "LineWidth", 15 );  
    
    %** Y and Z values swapped to account for Unity LH coordinate system**%
    % Set axis limits
    xlim([-1 1]); ylim([-1 1]); zlim([0 5]);
    
    % Set axis labels
    xlabel("spherePositionX [m]"); 
    ylabel("spherePositionZ [m]"); 
    zlabel("spherePositionY [m]");
    
    % Set axis ticks
    xticks(-5:1:5); yticks(-5:1:5); zticks(0:1:5);
    
    % Adjust plot camera properties
    campos([10, 8, 30]); camva(10);
    
    % Add box and grid
    box on; grid on; 
    
    %Print Timestamp
    % title("Time: " + time(i));
    legend("Time: " + time(i), "Location", "northwest");
%     legend("sphereX: " + sphereX(i), ...
%         "sphereY: " + sphereY(i), ...
%         "sphereZ: " + sphereZ(i), ...
%         "Time: " + time(i), "Location", "northwest");
    legend boxoff; 
    
    drawnow
    % wait a bit
    %pause(0.0001)          

end


