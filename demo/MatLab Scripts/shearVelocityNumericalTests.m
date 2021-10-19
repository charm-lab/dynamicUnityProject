% Shear Velocity Numerical Tests for dynamicUnityProject Virtual Environment
% All variables with subscript s --> sphere
% All variables with subscript f --> finger, either indew or thumb
% Vector Color Key:
%  - Black = Velocity Vector described in world space
%  - Cumquat = Relative velocity described in world space as 
%    v_finger - v_sphere
%  - Violet = Shear Velocity described in World Space

clear; close all; clc;

%% Test Case 1: vf > vs both vertical

% Velocities -- Values to be Changed
vs_Global = [0, 0, 1.5]; vf_Global = [0, 0, 2];

% Plotting
figure(1); improvePlot;

plotVelocities(vs_Global, vf_Global);

% title
title("Test Case 1: vf > vs, both vertical")

%% Test Case 2: vf < vs both vertical

% Velocities -- Values to be Changed
vs_Global = [0, 0, 2]; vf_Global = [0, 0, 1.5];

% Plotting
figure(2); improvePlot;

plotVelocities(vs_Global, vf_Global);

% title
title("Test Case 1: vf < vs, both vertical")


%% Test Case 3: |vf| > |vs| arbitray direction

% Velocities -- Values to be Changed
vs_Global = [0.5, 0.5, 1.5]; vf_Global = [1.5, 1, 2];

% Plotting
figure(3); improvePlot;

plotVelocities(vs_Global, vf_Global);

% title
title("Test Case 3: |vf| > |vs| arbitray direction")

%% Test Case 4: |vf| < |vs| arbitray direction

% Velocities -- Values to be Changed
vs_Global = [1.5, 1, 1]; vf_Global = -[0.5, 0.5, 1.5] ;

% Plotting
figure(4); improvePlot;

plotVelocities(vs_Global, vf_Global);

% title
title("Test Case 4: |vf| < |vs| arbitray direction")

%% Test Case 5: Randomized Vectors

% Velocities -- Values to be Changed
a = -2; b = 2;
vs_Global = [(b-a).*rand(3,1) + a]'; vf_Global = [(b-a).*rand(3,1) + a]';

% Plotting
figure(5); improvePlot;

plotVelocities(vs_Global, vf_Global);

% title
title("Test Case 5: Randomized Vectors")

%% Test Case 6: Random |vf|, |vs| = 0

% Velocities -- Values to be Changed
a = -2; b = 2;
vs_Global = [0.0001 0 0]; vf_Global = [(b-a).*rand(3,1) + a]';

% Plotting
figure(6); improvePlot;

plotVelocities(vs_Global, vf_Global);

% title
title("Test Case 6: Random |vf|, |vs| = 0")

%% Test Case 7:|vf| = 0, Random |vs|

% Velocities -- Values to be Changed
a = -2; b = 2;
vs_Global = [(b-a).*rand(3,1) + a]'; vf_Global = [0.0001 0 0];

% Plotting
figure(7); improvePlot;

plotVelocities(vs_Global, vf_Global);

% title
title("Test Case 7:|vf| = 0, Random |vs|")

%% Plotting Function
function plotVelocities(vs_Global, vf_Global)

% Sphere and Finger Dimensions
rs = 0.5; sphereCenter = [-0.5, 0, 0];
rf = 0.3; fingerCenter = [rs-rf, 0, 0];

vs = vs_Global + sphereCenter; vf = vf_Global + fingerCenter;
vRel = (vf_Global-vs_Global);

% Distance Vector
N = fingerCenter - sphereCenter;

% Sphere Center to Intersection Point
r1 = (rs^2 - rf^2 + norm(N)^2) / (2*norm(N));

% Intersection Point
pInt = sphereCenter + r1 * N/norm(N);

% Shear Velocity
shearVelocity = cross(N, cross(vRel,N)) / norm(N)^2;

% Define sphere surface
[X, Y, Z] = sphere;

% Plot sphere
surf(X*rs + sphereCenter(1), Y*rs + sphereCenter(2),...
    Z*rs + sphereCenter(3),'FaceColor', [0 0 1],'FaceAlpha',0.2);
hold on;

% Plot finger
surf(X*rf + fingerCenter(1), Y*rf + fingerCenter(2),...
    Z*rf + fingerCenter(3),'FaceColor', [1 0 0],'FaceAlpha',0.2);

% Plot sphere velocity vector - black
arrow3(sphereCenter,vs,'2',5);

% Plot finger velocity vector - black
arrow3(fingerCenter,vf,'2',5);

% Plot relative velocity vector - cumquat
arrow3(pInt, pInt+vRel,'q2',5);

% Plot shear velocity vector - Violet
arrow3(pInt, pInt+shearVelocity,'v2',5);

% Set axis labels
xlabel("X"); ylabel("Y"); zlabel("Z");
% Set axis limits
xlim([-3 3]); ylim([-3 3]); zlim([-3 3]);

clc;
end

