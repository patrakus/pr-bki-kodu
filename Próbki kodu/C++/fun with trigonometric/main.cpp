#include <SFML/Graphics.hpp>
#include <iostream>
#include <cmath>

float rotateRectangle(const sf::Vector2f& mousePos, const sf::RectangleShape& rectangle)
{
	//system("cls");

	//std::cout << mousePos.y << " " << mousePos.x << std::endl;

	return std::atan2f(mousePos.y - rectangle.getPosition().y, mousePos.x - rectangle.getPosition().x);
}

int main()
{
	sf::RenderWindow window(sf::VideoMode(800, 600), "SFML works!");

	sf::View mainCamera(sf::Vector2f(0.0f, 0.0f), sf::Vector2f(300.f, 200.f));
	window.setView(mainCamera);

	sf::RectangleShape rectangle(sf::Vector2f(50.f, 100.f));
	rectangle.setFillColor(sf::Color::Green);
	rectangle.setOrigin(sf::Vector2f(25.f, 50.f));

	sf::Clock timer;

	float pi = static_cast<float>(std::atan(1.0) * 4.0); // why use static_cast?


	while (window.isOpen())
	{
		float deltaTime = timer.restart().asSeconds();

		sf::Event event;
		while (window.pollEvent(event))
		{
			if (event.type == sf::Event::Closed)
				window.close();
			if (event.type == sf::Event::MouseMoved)
			{
				float angle = (rotateRectangle(window.mapPixelToCoords(sf::Mouse::getPosition(window)), rectangle) * 180.f) / pi;

				rectangle.setRotation(angle);
			}
		}

		window.clear();
		window.draw(rectangle);
		window.display();
	}

	return 0;
}