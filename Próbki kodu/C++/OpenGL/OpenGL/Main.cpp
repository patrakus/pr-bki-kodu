// Std. Includes
#include <string>

// GLEW
#include <GL/glew.h>

// GLFW
#include <GLFW/glfw3.h>

// GL includes
#include "ShaderProgram.h"
#include "Camera.h"
#include "Model.h"

// GLM Mathemtics
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>

// Other Libs
#include <SOIL\SOIL.h>

// Properties
GLuint screenWidth = 800, screenHeight = 600;

// Function prototypes
void key_callback(GLFWwindow* window, int key, int scancode, int action, int mode);
void mouse_callback(GLFWwindow* window, double xpos, double ypos);
void Do_Movement();

// Camera
Camera walkCamera(glm::vec3(0.0f, 1.f, 5.0f)); // klasa odpowiedzialna za obs³ugê kamery. More https://learnopengl.com/#!Getting-started/Camera
bool keys[1024];
GLfloat lastX = 400, lastY = 300;
bool firstMouse = true; 

GLfloat deltaTime = 0.0f;
GLfloat lastFrame = 0.0f;

// static camera
Camera staticCamera(glm::vec3(9.0f, 8.f, 10.0f), glm::vec3(0.0f, 1.0f, 0.0f), -120.f, -25.f);
GLboolean walkMode = true;

Camera* activeCamera = &walkCamera;

// wczytanie tekstury za pomoc¹ SOIL https://learnopengl.com/#!Getting-started/Textures

// The MAIN function, from here we start our application and run our Game loop

int main()
{
	// Init GLFW
	glfwInit();
	glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3); // Te dwie linijki ustalaj¹ z jakiej wersji OpenGL korzystamy
	glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
	glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE); // Informujemy ¿e korzystamy z nowoczesnych technik w OpenGL
	glfwWindowHint(GLFW_RESIZABLE, GL_FALSE); // Blokujemy mozliwoœc zmiany rozmiaru dla okna

	GLFWwindow* window = glfwCreateWindow(screenWidth, screenHeight, "LearnOpenGL", nullptr, nullptr); // Tworzenie okna
	glfwMakeContextCurrent(window); // stworzenie contextu dla OpenGL

	// Set the required callback functions
	glfwSetKeyCallback(window, key_callback);
	glfwSetCursorPosCallback(window, mouse_callback);

	// Options
	glfwSetInputMode(window, GLFW_CURSOR, GLFW_CURSOR_DISABLED); // Wy³¹czamy kursor

	// Initialize GLEW to setup the OpenGL Function pointers
	glewExperimental = GL_TRUE;
	glewInit();

	// Define the viewport dimensions
	glViewport(0, 0, screenWidth, screenHeight); //tworzy,y pogl¹d dla danego rozmiaru okna

	// Setup some OpenGL options
	glEnable(GL_DEPTH_TEST); // Informujemy OpenGL, aby wykonywa³ test g³êbii 

	ShaderProgram shader("VertexShader.txt", "FragmentShader.txt"); // W³asna klasa odpowiedzialna za wszelkie sprawy zwi¹zane z obs³uga shaderów
	// Load models
	// Model ourModel("nano/nanosuit.obj");
	Model box("objects/box/box.obj"); // Tworzymy model z podanego pliku
	Model cylinder18("objects/cylinder18/cylinder18.obj");
	Model cylinder32("objects/cylinder32/cylinder32.obj");
	Model sphere("objects/sphere/sphere.obj");
	Model torus("objects/torus/torus.obj");

	// Compound objects
	Model car("objects/ExtraObj/auto/auto.obj");

	// More advance objects
	Model cow("objects/ExtraObj/krowa/krowa.obj");
	Model trunk("objects/ExtraObj/pien/piendrzewa.obj");

	Model secondCow("objects/ExtraObj/krowa/krowa.obj");

	ShaderProgram secondShader("VertexShader.txt", "secondFragment.txt");

	// Draw in wireframe
	//glPolygonMode(GL_FRONT_AND_BACK, GL_LINE);

	// Game loop
	while (!glfwWindowShouldClose(window))
	{
		// Set frame time
		GLfloat currentFrame = glfwGetTime(); // Ta sekcja jest odpowiedzialna za obliczanie delty z czasu, aby móc j¹ poŸniej wykorzystaæ do pu¿niejszych obliczeñ do przemieszczania siê
		deltaTime = currentFrame - lastFrame;
		lastFrame = currentFrame;

		// Check and call events
		glfwPollEvents();
		Do_Movement(); // Sterowanie kamer¹

		// Clear the colorbuffer
		glClearColor(1.f, 1.f, 1.f, 1.0f);
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

		shader.use();   // <-- Don't forget this one!
						// Transformation matrices
		glm::mat4 projection = glm::perspective(activeCamera->Zoom, (float)screenWidth / (float)screenHeight, 0.1f, 100.0f);
		glm::mat4 view = activeCamera->GetViewMatrix();
		glUniformMatrix4fv(glGetUniformLocation(shader.Program, "projection"), 1, GL_FALSE, glm::value_ptr(projection));
		glUniformMatrix4fv(glGetUniformLocation(shader.Program, "view"), 1, GL_FALSE, glm::value_ptr(view));

		// Draw the loaded model
		glm::mat4 model;
		model = glm::translate(model, glm::vec3(-2.0f, 0.0, 0.0f));
		//model = glm::scale(model, glm::vec3(0.2f, 0.2f, 0.2f));	// It's a bit too big for our scene, so scale it down
		glUniformMatrix4fv(glGetUniformLocation(shader.Program, "model"), 1, GL_FALSE, glm::value_ptr(model));
		box.Draw(shader);

		model = glm::translate(model, glm::vec3(0.0f, 0.0, -2.0f)); // Translate in 2 units on -Z axis
		glUniformMatrix4fv(glGetUniformLocation(shader.Program, "model"), 1, GL_FALSE, glm::value_ptr(model));
		cylinder18.Draw(shader);

		model = glm::translate(model, glm::vec3(0.0f, 0.0, -2.0f)); // Translate again in 2 units on -Z axis
		glUniformMatrix4fv(glGetUniformLocation(shader.Program, "model"), 1, GL_FALSE, glm::value_ptr(model));
		cylinder32.Draw(shader);

		model = glm::translate(model, glm::vec3(0.0f, 0.0, -2.0f)); // Translate again in 2 units on -Z axis
		glUniformMatrix4fv(glGetUniformLocation(shader.Program, "model"), 1, GL_FALSE, glm::value_ptr(model));
		sphere.Draw(shader);

		model = glm::translate(model, glm::vec3(0.0f, 0.0, -2.0f)); // Translate again in 2 units on -Z axis
		glUniformMatrix4fv(glGetUniformLocation(shader.Program, "model"), 1, GL_FALSE, glm::value_ptr(model));
		torus.Draw(shader);

		model = glm::translate(model, glm::vec3(4.0f, 0.0, -2.0f)); // Translate again in 2 units on -Z axi and 4 units on X axi
		glUniformMatrix4fv(glGetUniformLocation(shader.Program, "model"), 1, GL_FALSE, glm::value_ptr(model));
		car.Draw(shader);

		model = glm::translate(model, glm::vec3(4.0f, 0.0, -2.0f)); // Translate again in 2 units on -Z axi and 4 units on X axi
		glUniformMatrix4fv(glGetUniformLocation(shader.Program, "model"), 1, GL_FALSE, glm::value_ptr(model));
		cow.Draw(shader);

		model = glm::translate(model, glm::vec3(4.0f, 0.0, -2.0f)); // Translate again in 2 units on -Z axi and 4 units on X axi
		glUniformMatrix4fv(glGetUniformLocation(shader.Program, "model"), 1, GL_FALSE, glm::value_ptr(model));
		trunk.Draw(shader);

		// change color over time
		GLfloat timeValue = glfwGetTime();
		GLfloat greenValue = (sin(timeValue) / 2) + 0.5f;
		secondShader.use();
		glUniform4f(glGetUniformLocation(secondShader.Program, "myColor"), 0.0f, greenValue, 0.0f, 1.0f);

		glUniformMatrix4fv(glGetUniformLocation(secondShader.Program, "projection"), 1, GL_FALSE, glm::value_ptr(projection));
		glUniformMatrix4fv(glGetUniformLocation(secondShader.Program, "view"), 1, GL_FALSE, glm::value_ptr(view));

		model = glm::translate(model, glm::vec3(4.0f, 0.0, -2.0f)); // Translate again in 2 units on -Z axi and 4 units on X axi
		glUniformMatrix4fv(glGetUniformLocation(secondShader.Program, "model"), 1, GL_FALSE, glm::value_ptr(model));
		secondCow.Draw(secondShader);


		// Swap the buffers
		glfwSwapBuffers(window);
	}

	glfwTerminate();
	return 0;
}

// Moves/alters the camera positions based on user input
void Do_Movement()
{
	if (walkMode)
	{
		// Camera controls
		if (keys[GLFW_KEY_W])
			walkCamera.ProcessKeyboard(FORWARD, deltaTime);
		if (keys[GLFW_KEY_S])
			walkCamera.ProcessKeyboard(BACKWARD, deltaTime);
		if (keys[GLFW_KEY_A])
			walkCamera.ProcessKeyboard(LEFT, deltaTime);
		if (keys[GLFW_KEY_D])
			walkCamera.ProcessKeyboard(RIGHT, deltaTime);
	}
}

/*
This is the function signature for keyboard key callback functions.

Parameters
    [in] window		The window that received the event.
    [in] key			The keyboard key that was pressed or released.
    [in] scancode	The system-specific scancode of the key.
    [in] action		GLFW_PRESS, GLFW_RELEASE or GLFW_REPEAT.
    [in] mods		Bit field describing which modifier keys were held down.
*/

// Is called whenever a key is pressed/released via GLFW
void key_callback(GLFWwindow* window, int key, int scancode, int action, int mode)
{
	//cout << key << endl;
	if (key == GLFW_KEY_ESCAPE && action == GLFW_PRESS)
		glfwSetWindowShouldClose(window, GL_TRUE);
	if (key >= 0 && key < 1024)
	{
		if (action == GLFW_PRESS)
			keys[key] = true;
		else if (action == GLFW_RELEASE)
			keys[key] = false;
	}
	if (key == GLFW_KEY_C && action == GLFW_RELEASE)
	{
		if (walkMode)
			activeCamera = &staticCamera;
		else
			activeCamera = &walkCamera;

		walkMode = walkMode ? false : true;
	}

}

void mouse_callback(GLFWwindow* window, double xpos, double ypos)
{
	if (walkMode)
	{
		if (firstMouse)
		{
			lastX = xpos;
			lastY = ypos;
			firstMouse = false;
		}

		GLfloat xoffset = xpos - lastX;
		GLfloat yoffset = lastY - ypos;  // Reversed since y-coordinates go from bottom to left

		lastX = xpos;
		lastY = ypos;

		walkCamera.ProcessMouseMovement(xoffset, yoffset);
	}
	
}