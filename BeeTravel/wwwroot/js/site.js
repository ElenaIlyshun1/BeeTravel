var size = 130,
	newsContent = $('.tour-content'),
	newsText = newsContent.text();

if (newsText.length > size) {
	newsContent.text(newsText.slice(0, size) + ' ...');
}